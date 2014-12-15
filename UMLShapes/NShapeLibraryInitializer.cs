using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLShapes
{
    /// <summary>
    /// Initializes the shape library by registering the library itself and all of its available shape types.
    /// </summary>
    public static class NShapeLibraryInitializer
    {

        public static void Initialize(IRegistrar registrar)
        {
            // Register library
            registrar.RegisterLibrary(libraryName, preferredRepositoryVersion);

            // Create shape type instance for "MyShape" ...
            ShapeType myShapeType = new ShapeType(
                    "UMLShape",                                                         // The name of the shape type, used 
                    libraryName,                                                 // The name of the library the shape type belongs to
                    categoryName,                                                 // Name of the default toolbox category
                    delegate(ShapeType shapeType, Template t)
                    {        // A 'CreateShapeDelegate' method that constructs the shape
                        return new UMLShape(shapeType, t);
                    },
                    UMLShape.GetPropertyDefinitions,                         // Static method that provides property definitions
                    true);                                                        // Specifies whether templates can be created from this shape
            // ... and register it
            registrar.RegisterShapeType(myShapeType);
        }

        private const string libraryName = "UMLShapes";
        private const string categoryName = "MyShapes";
        private const int preferredRepositoryVersion = 5;
    }
}
