using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services.Catalogs.Domain;

public static class AppCts
{
    // Error Messages
    public static class Errors
    {
        // Maps
        public static class Maps
        {
            public const string NotFound = "The Map is not found. Please try again.";
            public const string AlreadyExists = "The Map is already existed.";
        }
    }

    // Contains the directory of the file to seed the data
    public static class SeederFilePaths
    {
        // Get the folder at runtime
        private const string JsonFolderPath = "Data/SeederFiles";

        // Each json file path
        public static readonly string MasterSubjectFilePath = Path.Combine(JsonFolderPath, "MasterSubject.json");
        public static readonly string SubjectFilePath = Path.Combine(JsonFolderPath, "Subject.json");
        public static readonly string ActivityTypeFilePath = Path.Combine(JsonFolderPath, "ActivityType.json");
        public static readonly string MapFilePath = Path.Combine(JsonFolderPath, "Map.json");
        public static readonly string MapObjectFilePath = Path.Combine(JsonFolderPath, "MapObject.json");
        public static readonly string TaskLocationFilePath = Path.Combine(JsonFolderPath, "TaskLocation.json");
        public static readonly string ObjectActivityTypeFilePath = Path.Combine(JsonFolderPath, "ObjectActivityType.json");
        public static readonly string ObjectLocationFilePath = Path.Combine(JsonFolderPath, "ObjectLocation.json");
    }

    public static class ProdSeederFilePaths
    {
        // Get the folder at runtime
        private const string JsonFolderPath = "Data/SeederFiles/Production";

        // Each json file path
        public static readonly string MasterSubjectFilePath = Path.Combine(JsonFolderPath, "MasterSubject.json");
        public static readonly string SubjectFilePath = Path.Combine(JsonFolderPath, "Subject.json");
        public static readonly string ActivityTypeFilePath = Path.Combine(JsonFolderPath, "ActivityType.json");
        public static readonly string MapFilePath = Path.Combine(JsonFolderPath, "Map.json");
        public static readonly string MapObjectFilePath = Path.Combine(JsonFolderPath, "MapObject.json");
        public static readonly string TaskLocationFilePath = Path.Combine(JsonFolderPath, "TaskLocation.json");
        public static readonly string ObjectActivityTypeFilePath = Path.Combine(JsonFolderPath, "ObjectActivityType.json");
        public static readonly string ObjectLocationFilePath = Path.Combine(JsonFolderPath, "ObjectLocation.json");
    }

}
