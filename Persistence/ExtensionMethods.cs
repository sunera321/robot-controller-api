// FastMember is the ORM library that automatically maps database columns to C# properties
using FastMember;

// Npgsql for NpgsqlDataReader (the database row reader)
using Npgsql;

namespace robot_controller_api.Persistence
{
    // "static class" because extension methods must live in a static class
    public static class ExtensionMethods
    {
        // This is an EXTENSION METHOD — it adds a new method called MapTo()
        // to the NpgsqlDataReader class, which we cannot modify directly
        // "this NpgsqlDataReader dr" means: this method belongs to NpgsqlDataReader
        // T = the type of object to fill (e.g. RobotCommand, Map)
        public static void MapTo<T>(this NpgsqlDataReader dr, T entity)
        {
            // Safety check — if entity is null, throw a clear error message
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // TypeAccessor is FastMember's tool to quickly read/write C# object properties
            // It is much faster than normal .NET Reflection
            // entity.GetType() tells FastMember which class we are working with
            var fastMember = TypeAccessor.Create(entity.GetType());

            // GetMembers() returns all properties of the class (Id, Name, IsMoveCommand, etc.)
            // .Select(x => x.Name) gets just the property names as strings
            // .ToHashSet() stores them in a HashSet for super-fast lookup (O(1) speed)
            // StringComparer.OrdinalIgnoreCase = case-insensitive matching
            //   so "IsMoveCommand" matches "ismovecommand" from the database
            var props = fastMember.GetMembers()
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Loop through every column in the current database row
            // dr.FieldCount = total number of columns returned by SQL
            for (int i = 0; i < dr.FieldCount; i++)
            {
                // dr.GetName(i) = the column name from the database e.g. "ismovecommand"
                // props.FirstOrDefault(...) searches for a matching C# property name
                // The comparison ignores case so "ismovecommand" matches "IsMoveCommand"
                var prop = props.FirstOrDefault(x =>
                    x.Equals(dr.GetName(i), StringComparison.OrdinalIgnoreCase));

                // If a matching property was found (prop is not empty)
                if (!string.IsNullOrEmpty(prop))
                {
                    // fastMember[entity, prop] = set the property value on the object
                    // dr.IsDBNull(i) checks if the database value is null
                    //   if null → store null in C#
                    //   if not null → dr.GetValue(i) reads the actual value
                    fastMember[entity, prop] = dr.IsDBNull(i) ? null : dr.GetValue(i);
                }
            }
            // After this loop, all matching columns are automatically set on the entity object!
        }
    }
}