using Dataweb.NShape.Advanced;
using System;
using System.Collections.Generic;
using System.Drawing;
using Dataweb.NShape;
using Dataweb.NShape.Commands;
using System.ComponentModel;
using System.Drawing.Design;

namespace UMLShapes
{
    #region Commands for editing UMLShape columns

    public abstract class UMLShapeColumnCommand : Command
    {

        protected UMLShapeColumnCommand(UMLShape shape, string columnText)
            : base()
        {
            this.shape = shape;
            this.columnText = columnText;
        }


        protected Shape Shape
        {
            get { return shape; }
        }

        protected string ColumnText
        {
            get { return columnText; }
        }


        public override Permission RequiredPermission
        {
            get { return Permission.Data; }
        }


        /// <override></override>
        protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception)
        {
            if (securityManager == null) throw new ArgumentNullException("securityManager");
            bool isGranted = securityManager.IsGranted(RequiredPermission, shape.SecurityDomainName);
            exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
            return isGranted;
        }


        protected UMLShape shape;
        private string columnText;
    }


    public class AddColumnCommand : UMLShapeColumnCommand
    {

        public AddColumnCommand(UMLShape shape, string columnText)
            : base(shape, columnText)
        {
            base.description = string.Format("Add column to {0}", shape.Type.Name);
        }


        #region ICommand Members

        /// <override></override>
        public override void Execute()
        {
            shape.AddColumn(ColumnText);
            if (Repository != null) Repository.Update(Shape);
        }


        /// <override></override>
        public override void Revert()
        {
            shape.RemoveColumn(ColumnText);
            if (Repository != null) Repository.Update(Shape);
        }

        #endregion

    }


    public class InsertColumnCommand : UMLShapeColumnCommand
    {

        public InsertColumnCommand(UMLShape shape, int beforeColumnIndex, string columnText)
            : base(shape, columnText)
        {
            base.description = string.Format("Insert new column in {0}", shape.Type.Name);
            this.beforeIndex = beforeColumnIndex;
        }


        #region ICommand Members

        /// <override></override>
        public override void Execute()
        {
            shape.AddColumn(shape.GetCaptionText(shape.CaptionCount - 1));
            for (int i = shape.CaptionCount - 2; i > beforeIndex; --i)
                shape.SetCaptionText(i, shape.GetCaptionText(i - 1));
            shape.SetCaptionText(beforeIndex, ColumnText);
            if (Repository != null) Repository.Update(Shape);
        }


        /// <override></override>
        public override void Revert()
        {
            for (int i = shape.CaptionCount - 1; i > beforeIndex; --i)
                shape.SetCaptionText(i - 1, shape.GetCaptionText(i));
            // The shape's Text does count as caption but not as column, that's why CaptionCount-2.
            shape.RemoveColumnAt(shape.CaptionCount - 2);
            if (Repository != null) Repository.Update(Shape);
        }

        #endregion


        private int beforeIndex;
    }


    public class EditColumnCommand : UMLShapeColumnCommand
    {

        public EditColumnCommand(UMLShape shape, int columnIndex, string columnText)
            : base(shape, columnText)
        {
            base.description = string.Format("Edit column '{0}' in {1}", columnText, shape.Type.Name);
            this.oldColumnText = shape.ColumnNames[columnIndex];
            this.columnIndex = columnIndex;
        }


        #region ICommand Members

        /// <override></override>
        public override void Execute()
        {
            string[] columns = new string[shape.ColumnNames.Length];
            Array.Copy(shape.ColumnNames, columns, shape.ColumnNames.Length);
            columns[columnIndex] = ColumnText;

            shape.ColumnNames = columns;
            if (Repository != null) Repository.Update(Shape);
        }


        /// <override></override>
        public override void Revert()
        {
            string[] columns = new string[shape.ColumnNames.Length];
            Array.Copy(shape.ColumnNames, columns, shape.ColumnNames.Length);
            columns[columnIndex] = oldColumnText;

            shape.ColumnNames = columns;
            if (Repository != null) Repository.Update(Shape);
        }

        #endregion


        private string oldColumnText;
        private int columnIndex;
    }


    public class RemoveColumnCommand : UMLShapeColumnCommand
    {

        public RemoveColumnCommand(UMLShape shape, int removeColumnIndex, string columnText)
            : base(shape, columnText)
        {
            base.description = string.Format("Remove column '{0}' from {1}", columnText, shape.Type.Name);
            this.removeIndex = removeColumnIndex;
        }


        #region ICommand Members

        /// <override></override>
        public override void Execute()
        {
            int maxCaptionIdx = shape.CaptionCount - 1;
            for (int i = removeIndex; i < maxCaptionIdx; ++i)
                shape.SetCaptionText(i, shape.GetCaptionText(i + 1));
            // The shape's Text does count as caption but not as column, that's why maxCaptionIdx - 1.
            shape.RemoveColumnAt(maxCaptionIdx - 1);
            if (Repository != null) Repository.Update(Shape);
        }


        /// <override></override>
        public override void Revert()
        {
            shape.AddColumn(shape.GetCaptionText(shape.CaptionCount - 1));
            for (int i = shape.CaptionCount - 2; i > removeIndex; --i)
                shape.SetCaptionText(i, shape.GetCaptionText(i - 1));
            shape.SetCaptionText(removeIndex, ColumnText);
            if (Repository != null) Repository.Update(Shape);
        }

        #endregion


        private int removeIndex;
    }

    #endregion

    public class UMLShape : RectangleBase
    {

       
        protected internal UMLShape(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}

        public override Shape Clone()
        {
            Shape result = new UMLShape(Type, this.Template);
            result.CopyFrom(this);
            return result;
        }

        static public UMLShape CreateInstance(ShapeType shapeType, Template template)
        {
            return new UMLShape(shapeType, template);
        }

        #region ICaptionedShape Implementation

        /// <override></override>
        public override int CaptionCount { get { return base.CaptionCount + columnCaptions.Count; } }


        /// <override></override>
        public override bool GetCaptionBounds(int index, out Point topLeft, out Point topRight, out Point bottomRight, out Point bottomLeft)
        {
            if (index < base.CaptionCount)
                return base.GetCaptionBounds(index, out topLeft, out topRight, out bottomRight, out bottomLeft);
            else
            {
                int idx = index - 1;
                Rectangle captionBounds;
                CalcCaptionBounds(index, out captionBounds);
                Geometry.TransformRectangle(Center, Angle, captionBounds, out topLeft, out topRight, out bottomRight, out bottomLeft);
                return (Geometry.ConvexPolygonContainsPoint(columnFrame, bottomLeft.X, bottomLeft.Y)
                    && Geometry.ConvexPolygonContainsPoint(columnFrame, bottomRight.X, bottomRight.Y));
            }
        }


        /// <override></override>
        public override bool GetCaptionTextBounds(int index, out Point topLeft, out Point topRight, out Point bottomRight, out Point bottomLeft)
        {
            if (index < base.CaptionCount)
            {
                return base.GetCaptionTextBounds(index, out topLeft, out topRight, out bottomRight, out bottomLeft);
            }
            else
            {
                int idx = index - 1;
                Rectangle bounds;
                CalcCaptionBounds(index, out bounds);
                bounds = columnCaptions[idx].CalculateTextBounds(bounds, ColumnCharacterStyle, ColumnParagraphStyle, DisplayService);
                Geometry.TransformRectangle(Center, Angle, bounds, out topLeft, out topRight, out bottomRight, out bottomLeft);
                return (Geometry.QuadrangleContainsPoint(columnFrame[0], columnFrame[1], columnFrame[2], columnFrame[3], topLeft.X, topLeft.Y)
                    && Geometry.QuadrangleContainsPoint(columnFrame[0], columnFrame[1], columnFrame[2], columnFrame[3], bottomRight.X, bottomRight.Y));
            }
        }


        /// <override></override>
        public override string GetCaptionText(int index)
        {
            if (index < base.CaptionCount)
                return base.GetCaptionText(index);
            else
                return columnCaptions[index - 1].Text;
        }


        /// <override></override>
        public override ICharacterStyle GetCaptionCharacterStyle(int index)
        {
            if (index < base.CaptionCount)
                return base.GetCaptionCharacterStyle(index);
            else return ColumnCharacterStyle;
        }


        /// <override></override>
        public override IParagraphStyle GetCaptionParagraphStyle(int index)
        {
            if (index < base.CaptionCount)
                return base.GetCaptionParagraphStyle(index);
            else return ColumnParagraphStyle;
        }


        /// <override></override>
        public override void SetCaptionText(int index, string text)
        {
            if (index < base.CaptionCount)
                base.SetCaptionText(index, text);
            else
            {
                Invalidate();
                columnCaptions[index - 1].Text = text;
                InvalidateDrawCache();
                Invalidate();
            }
        }


        /// <override></override>
        public override void SetCaptionCharacterStyle(int index, ICharacterStyle characterStyle)
        {
            if (index < base.CaptionCount)
                base.SetCaptionCharacterStyle(index, characterStyle);
            else
            {
                int idx = index - 1;
                // Create if needed
                if (columnCharacterStyles == null)
                    columnCharacterStyles = new SortedList<int, ICharacterStyle>(1);
                // Set private style for a single caption
                if (characterStyle != ColumnCharacterStyle)
                {
                    if (!columnCharacterStyles.ContainsKey(idx))
                        columnCharacterStyles.Add(idx, characterStyle);
                    else columnCharacterStyles[idx] = characterStyle;
                }
                else
                {
                    if (columnCharacterStyles != null)
                    {
                        if (columnCharacterStyles.ContainsKey(idx))
                            columnCharacterStyles.Remove(idx);
                        // Delete if not needed any more
                        if (columnCharacterStyles.Count == 0)
                            columnCharacterStyles = null;
                    }
                }
            }
        }


        /// <override></override>
        public override void SetCaptionParagraphStyle(int index, IParagraphStyle paragraphStyle)
        {
            if (index < base.CaptionCount)
                base.SetCaptionParagraphStyle(index, paragraphStyle);
            else
            {
                int idx = index - 1;
                // Create if needed
                if (columnParagraphStyles == null)
                    columnParagraphStyles = new SortedList<int, IParagraphStyle>(1);
                // Set private style for a single caption
                if (paragraphStyle != ColumnParagraphStyle)
                {
                    if (!columnParagraphStyles.ContainsKey(idx))
                        columnParagraphStyles.Add(idx, paragraphStyle);
                    else columnParagraphStyles[idx] = paragraphStyle;
                }
                else
                {
                    if (columnParagraphStyles != null)
                    {
                        if (columnParagraphStyles.ContainsKey(idx))
                            columnParagraphStyles.Remove(idx);
                        // Delete if not needed any longer
                        if (columnParagraphStyles.Count == 0)
                            columnParagraphStyles = null;
                    }
                }
            }
        }


        /// <override></override>
        public override void ShowCaptionText(int index)
        {
            if (index < base.CaptionCount)
                base.ShowCaptionText(index);
            else
                columnCaptions[index - 1].IsVisible = true;
        }


        /// <override></override>
        public override void HideCaptionText(int index)
        {
            if (index < base.CaptionCount)
                base.HideCaptionText(index);
            else
            {
                columnCaptions[index - 1].IsVisible = false;
                Invalidate();
            }
        }

        #endregion

        #region IEntity Implementation

        /// <override></override>
        protected override void SaveFieldsCore(IRepositoryWriter writer, int version)
        {
            base.SaveFieldsCore(writer, version);
            writer.WriteStyle(ColumnBackgroundColorStyle);
            writer.WriteStyle(ColumnCharacterStyle);
            writer.WriteStyle(ColumnParagraphStyle);
            writer.WriteInt32(ColumnNames.Length);
        }


        /// <override></override>
        protected override void LoadFieldsCore(IRepositoryReader reader, int version)
        {
            base.LoadFieldsCore(reader, version);
            ColumnBackgroundColorStyle = reader.ReadColorStyle();
            ColumnCharacterStyle = reader.ReadCharacterStyle();
            ColumnParagraphStyle = reader.ReadParagraphStyle();
            int colCnt = reader.ReadInt32();
            if (columnNames == null) columnNames = new string[colCnt];
            else Array.Resize(ref columnNames, colCnt);
        }


        /// <override></override>
        protected override void SaveInnerObjectsCore(string propertyName, IRepositoryWriter writer, int version)
        {
            if (propertyName == attrNameColumns)
            {
                writer.BeginWriteInnerObjects();
                int cnt = CaptionCount;
                for (int i = 1; i < cnt; ++i)
                {	// Skip first caption (title)
                    writer.BeginWriteInnerObject();
                    writer.WriteInt32(i - 1);
                    writer.WriteString(GetCaptionText(i));
                    writer.EndWriteInnerObject();
                }
                writer.EndWriteInnerObjects();
            }
            else base.SaveInnerObjectsCore(propertyName, writer, version);
        }


        /// <override></override>
        protected override void LoadInnerObjectsCore(string propertyName, IRepositoryReader reader, int version)
        {
            if (propertyName == attrNameColumns)
            {
                reader.BeginReadInnerObjects();
                while (reader.BeginReadInnerObject())
                {
                    int colIdx = reader.ReadInt32();
                    string colName = reader.ReadString();
                    reader.EndReadInnerObject();
                    InsertColumn(colIdx, colName);
                }
                reader.EndReadInnerObjects();
            }
            else base.LoadInnerObjectsCore(propertyName, reader, version);
        }


        /// <summary>
        /// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.SoftwareArchitectureShapes.EntitySymbol" />.
        /// </summary>
        public static new IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
        {
            foreach (EntityPropertyDefinition pi in RectangleBase.GetPropertyDefinitions(version))
                yield return pi;
            yield return new EntityFieldDefinition("ColumnBackgroundColorStyle", typeof(object));
            yield return new EntityFieldDefinition("ColumnCharacterStyle", typeof(object));
            yield return new EntityFieldDefinition("ColumnParagraphStyle", typeof(object));
            yield return new EntityFieldDefinition("ColumnCount", typeof(int));
            yield return new EntityInnerObjectsDefinition(attrNameColumns, attrNameColumn, columnAttrNames, columnAttrTypes);
        }

        #endregion

        #region Fields

        protected const int PropertyIdColumnBackgroundColorStyle = 9;
        protected const int PropertyIdColumnCharacterStyle = 10;
        protected const int PropertyIdColumnParagraphStyle = 11;

        private const string attrNameColumns = "TableColumns";
        private const string attrNameColumn = "Column";
        private static string[] columnAttrNames = new string[] { "ColumnIndex", "ColumnName" };
        private static Type[] columnAttrTypes = new Type[] { typeof(int), typeof(string) };

        private string[] columnNames = new string[0];
        private List<Caption> columnCaptions = new List<Caption>(1);
        private List<Rectangle> columnBounds = new List<Rectangle>(0);
        private SortedList<int, ICharacterStyle> columnCharacterStyles = null;
        private SortedList<int, IParagraphStyle> columnParagraphStyles = null;
        private Point[] columnControlPoints = new Point[0];
        private IColorStyle privateColumnBackgroundColorStyle = null;
        private ICharacterStyle privateColumnCharacterStyle = null;
        private IParagraphStyle privateColumnParagraphStyle = null;

        private Rectangle rectBuffer = Rectangle.Empty;
        //private Point[] pointBuffer = new Point[4];
        private Point[] columnFrame = new Point[4];
        private Point[] upperScrollArrow = new Point[3];
        private Point[] lowerScrollArrow = new Point[3];

        #endregion

        #region [Public] Properties

        [CategoryAppearance()]
        [Description("Defines the appearence of the shape's interior.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
        [PropertyMappingId(PropertyIdColumnBackgroundColorStyle)]
        [RequiredPermission(Permission.Present)]
        public virtual IColorStyle ColumnBackgroundColorStyle
        {
            get { return privateColumnBackgroundColorStyle ?? ((UMLShape)Template.Shape).ColumnBackgroundColorStyle; }
            set
            {
                privateColumnBackgroundColorStyle = (Template != null && Template.Shape is UMLShape && value == ((UMLShape)Template.Shape).ColumnBackgroundColorStyle) ? null : value;
                Invalidate();
            }
        }


        [Category("Text Appearance")]
        [Description("Determines the style of the shape's column names.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
        [PropertyMappingId(PropertyIdColumnCharacterStyle)]
        [RequiredPermission(Permission.Present)]
        public ICharacterStyle ColumnCharacterStyle
        {
            get { return privateColumnCharacterStyle ?? ((UMLShape)Template.Shape).ColumnCharacterStyle; }
            set
            {
                Invalidate();
                privateColumnCharacterStyle = (Template != null && Template.Shape is UMLShape && value == ((UMLShape)Template.Shape).ColumnCharacterStyle) ? null : value;
                InvalidateDrawCache();
                Invalidate();
            }
        }


        [Category("Text Appearance")]
        [Description("Determines the layout of the shape's column names.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
        [PropertyMappingId(PropertyIdColumnParagraphStyle)]
        [RequiredPermission(Permission.Present)]
        public IParagraphStyle ColumnParagraphStyle
        {
            get
            {
                return privateColumnParagraphStyle ?? ((UMLShape)Template.Shape).ColumnParagraphStyle;
            }
            set
            {
                Invalidate();
                privateColumnParagraphStyle = (Template != null && Template.Shape is UMLShape && value == ((UMLShape)Template.Shape).ColumnParagraphStyle) ? null : value;
                InvalidateDrawCache();
                Invalidate();
            }
        }


        [Category("Text Layout")]
        [Description("The column names of this table.")]
        [RequiredPermission(Permission.Present)]
        [TypeConverter("Dataweb.NShape.WinFormsUI.TextTypeConverter")]
        [Editor("Dataweb.NShape.WinFormsUI.TextUITypeEditor", typeof(UITypeEditor))]
        public string[] ColumnNames
        {
            get
            {
                if (columnNames == null || columnNames.Length != columnCaptions.Count)
                    columnNames = new string[columnCaptions.Count];
                for (int i = columnCaptions.Count - 1; i >= 0; --i)
                {
                    if (columnNames[i] != columnCaptions[i].Text)
                        columnNames[i] = columnCaptions[i].Text;
                }
                return columnNames;
            }
            set
            {
                if (value == null) throw new ArgumentNullException();
                Invalidate();

                // Remove columns that are no longer needed
                int valueCnt = value.Length;
                if (columnNames.Length > valueCnt)
                {
                    for (int i = columnNames.Length - 1; i >= valueCnt; --i)
                        RemoveColumnAt(i);
                }
                // Replace existing and add new columns
                for (int i = 0; i < valueCnt; ++i)
                {
                    if (i < columnNames.Length)
                    {
                        columnCaptions[i].Text =
                            columnNames[i] = value[i];
                    }
                    else AddColumn(value[i]);
                }

                InvalidateDrawCache();
                Invalidate();
            }
        }
        #endregion

        #region Caption objects stuff

        public void AddColumn(string columnName)
        {
            columnCaptions.Add(new Caption(columnName));
            Array.Resize(ref columnControlPoints, columnControlPoints.Length + 2);
            Array.Resize(ref columnNames, columnNames.Length + 1);
            columnNames[columnNames.Length - 1] = columnName;
            InvalidateDrawCache();
            Invalidate();
        }


        public void InsertColumn(int index, string columnName)
        {
            columnCaptions.Insert(index, new Caption(columnName));
            Array.Resize(ref columnControlPoints, columnControlPoints.Length + 2);
            Array.Resize(ref columnNames, columnCaptions.Count);
            for (int i = columnCaptions.Count - 1; i >= 0; --i)
                columnNames[i] = columnCaptions[i].Text;
            InvalidateDrawCache();
            Invalidate();
        }


        public void RemoveColumn(string columnName)
        {
            for (int i = columnCaptions.Count - 1; i >= 0; --i)
            {
                if (columnName.Equals(columnCaptions[i].Text, StringComparison.InvariantCulture))
                {
                    RemoveColumnAt(i);
                    break;
                }
            }
            InvalidateDrawCache();
            Invalidate();
        }


        public void RemoveColumnAt(int index)
        {
            if (index < 0 || index > columnCaptions.Count)
                throw new ArgumentOutOfRangeException("index");
            // Check whether connection points are not connected
            const String stillConnectedMsg = "Cannot remove connection point {0}: Other shapes are still connected to this point.";
            ControlPointId leftCtrlPtId = GetControlPointId(base.ControlPointCount + (2 * index));
            if (IsConnected(leftCtrlPtId, null) != ControlPointId.None)
                throw new NShapeException(stillConnectedMsg, leftCtrlPtId);
            ControlPointId rightCtrlPtId = GetControlPointId(base.ControlPointCount + (2 * index) + 1);
            if (IsConnected(rightCtrlPtId, null) != ControlPointId.None)
                throw new NShapeException(stillConnectedMsg, rightCtrlPtId);

            // Remove caption
            columnCaptions.RemoveAt(index);
            if (index < columnControlPoints.Length - 2)
                Array.Copy(columnControlPoints, index + 2, columnControlPoints, index, columnControlPoints.Length - index - 2);
            Array.Resize(ref columnControlPoints, columnControlPoints.Length - 2);
            if (index < columnNames.Length - 1)
                Array.Copy(columnNames, index + 1, columnNames, index, columnNames.Length - index - 1);
            Array.Resize(ref columnNames, columnCaptions.Count);

            InvalidateDrawCache();
            Invalidate();
        }


        public void ClearColumns()
        {
            columnCaptions.Clear();
            Array.Resize<Point>(ref columnControlPoints, 0);
            Array.Resize(ref columnNames, 0);
            InvalidateDrawCache();
            Invalidate();
        }

        #endregion

        /// <override></override>
        public override IEnumerable<MenuItemDef> GetMenuItemDefs(int mouseX, int mouseY, int range)
        {
            // return actions of base class
            IEnumerator<MenuItemDef> enumerator = GetBaseActions(mouseX, mouseY, range);
            while (enumerator.MoveNext()) yield return enumerator.Current;
            // return own actions

            string newColumnTxt = string.Format("Column {0}", CaptionCount);
            int captionIdx = -1;
            if (ContainsPoint(mouseX, mouseY))
            {
                Point tl, tr, bl, br;
                for (int i = columnCaptions.Count - 1; i >= 0; --i)
                {
                    // +1 because Text Property is Caption '0'
                    GetCaptionBounds(i + 1, out tl, out tr, out br, out bl);
                    if (Geometry.QuadrangleContainsPoint(tl, tr, br, bl, mouseX, mouseY))
                    {
                        // +1 because Text Property is Caption '0'
                        captionIdx = i + 1;
                        break;
                    }
                }
            }
}

        private IEnumerator<MenuItemDef> GetBaseActions(int mouseX, int mouseY, int range)
        {
            return base.GetMenuItemDefs(mouseX, mouseY, range).GetEnumerator();
        }

    }
}
