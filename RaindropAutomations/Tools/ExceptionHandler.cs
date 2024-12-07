namespace RaindropAutomations.Tools
{
    public class ExceptionHandler
    {
        public static void ThrowIfAnyNull(string methodName, params (object Value, string Name)[] items)
        {
            var nullItems = items.Where(i => i.Value == null)
                                 .Select(i => i.Name)
                                 .ToArray();

            if (nullItems.Any())
                throw new InvalidOperationException($"In method '{methodName}', the following values are null: {string.Join(", ", nullItems)}");
        }
    }
}
