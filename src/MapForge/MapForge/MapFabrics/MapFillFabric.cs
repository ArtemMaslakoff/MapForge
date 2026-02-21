using MapForge.Maps.Core;

namespace MapForge.MapFabrics
{
    public static class MapFillFabric
    {
        public static void FullFill<T>(IMap map, string parameterName, T value)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            if (string.IsNullOrEmpty(parameterName)) throw new ArgumentNullException(nameof(parameterName));

            var param = map.ParameterDefinitions.FirstOrDefault(p => p.Name == parameterName);
            if (param == null)
                throw new ArgumentException($"Parameter '{parameterName}' is not defined on this map.", nameof(parameterName));
            if (!param.IsValidObject(value))
                throw new ArgumentException($"Value is not valid for parameter '{parameterName}'.", nameof(value));

            foreach (ICell cell in map.GetAllCells())
            {
                cell.SetValue(parameterName, value);
            }
        }
        public static void FullDefaultFill(IMap map, string parameterName)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            if (string.IsNullOrEmpty(parameterName)) throw new ArgumentNullException(nameof(parameterName));

            var param = map.ParameterDefinitions.FirstOrDefault(p => p.Name == parameterName);
            if (param == null)
                throw new ArgumentException($"Parameter '{parameterName}' is not defined on this map.", nameof(parameterName));

            object? defaultValue = param.DefaultValue;
            foreach (ICell cell in map.GetAllCells())
            {
                cell.SetValue(parameterName, defaultValue);
            }
        }
        public static void FullDefaultFill(IMap map)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));

            foreach (ICell cell in map.GetAllCells())
            {
                foreach (IParameterDefinition param in map.ParameterDefinitions)
                {
                    cell.SetValue(param.Name, param.DefaultValue);
                }
            }
        }

        public static void FillCenter<T>(IMap map, string parameterName, T value, int extent)
        {
            ValidateParameter(map, parameterName, value);
            int[] center = GetCenter(map);
            foreach (ICell cell in map.GetAllCells())
            {
                int[] coords = cell.GetCoordinates();
                if (DistanceFromCenter(coords, center) <= extent)
                {
                    cell.SetValue(parameterName, value);
                }
            }
        }
        public static void FillBorder<T>(IMap map, string parameterName, T value, int thickness)
        {
            if (thickness <= 0) throw new ArgumentOutOfRangeException(nameof(thickness));
            ValidateParameter(map, parameterName, value);
            foreach (ICell cell in map.GetAllCells())
            {
                if (IsOnBorder(cell.GetCoordinates(), map, thickness))
                {
                    cell.SetValue(parameterName, value);
                }
            }
        }
        public static void FillInterior<T>(IMap map, string parameterName, T value, int margin)
        {
            if (margin < 0) throw new ArgumentOutOfRangeException(nameof(margin));
            ValidateParameter(map, parameterName, value);
            foreach (ICell cell in map.GetAllCells())
            {
                int[] c = cell.GetCoordinates();
                bool inside = true;
                for (int d = 0; d < c.Length; d++)
                {
                    int len = map.GetLength(d);
                    if (c[d] < margin || c[d] >= len - margin) { inside = false; break; }
                }
                if (inside)
                    cell.SetValue(parameterName, value);
            }
        }
        public static void FillCorner<T>(IMap map, string parameterName, T value, int size, int[] cornerMask)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            if (cornerMask == null || cornerMask.Length != map.GetDimensionCount())
                throw new ArgumentException("cornerMask length must match map dimension count.", nameof(cornerMask));
            ValidateParameter(map, parameterName, value);
            int dims = map.GetDimensionCount();
            int[] min = new int[dims], max = new int[dims];
            for (int d = 0; d < dims; d++)
            {
                int len = map.GetLength(d);
                if (cornerMask[d] == 0) { min[d] = 0; max[d] = Math.Min(size, len) - 1; }
                else { max[d] = len - 1; min[d] = Math.Max(0, len - size); }
            }
            foreach (ICell cell in map.GetAllCells())
            {
                int[] c = cell.GetCoordinates();
                bool inCorner = true;
                for (int d = 0; d < dims; d++)
                    if (c[d] < min[d] || c[d] > max[d]) { inCorner = false; break; }
                if (inCorner) cell.SetValue(parameterName, value);
            }
        }
        public static void FillCorners<T>(IMap map, string parameterName, T value, int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
            ValidateParameter(map, parameterName, value);
            int dims = map.GetDimensionCount();
            int cornerCount = 1 << dims; // 2^dims
            foreach (ICell cell in map.GetAllCells())
            {
                int[] c = cell.GetCoordinates();
                for (int k = 0; k < cornerCount; k++)
                {
                    bool inThisCorner = true;
                    for (int d = 0; d < dims; d++)
                    {
                        int len = map.GetLength(d);
                        bool fromEnd = ((k >> d) & 1) == 1;
                        int lo = fromEnd ? Math.Max(0, len - size) : 0;
                        int hi = fromEnd ? len - 1 : Math.Min(size, len) - 1;
                        if (c[d] < lo || c[d] > hi) { inThisCorner = false; break; }
                    }
                    if (inThisCorner) { cell.SetValue(parameterName, value); break; }
                }
            }
        }
        public static void FillBand<T>(IMap map, string parameterName, T value, int axis, int start, int length)
        {
            if (axis < 0 || axis >= map.GetDimensionCount())
                throw new ArgumentOutOfRangeException(nameof(axis));
            int len = map.GetLength(axis);
            if (start < 0 || length <= 0 || start + length > len)
                throw new ArgumentOutOfRangeException(nameof(start));
            ValidateParameter(map, parameterName, value);
            int end = start + length;
            foreach (ICell cell in map.GetAllCells())
            {
                int[] c = cell.GetCoordinates();
                if (c[axis] >= start && c[axis] < end)
                    cell.SetValue(parameterName, value);
            }
        }

        private static int DistanceFromCenter(int[] coords, int[] center)
        {
            int max = 0;
            for (int i = 0; i < coords.Length; i++)
            {
                int d = Math.Abs(coords[i] - center[i]);
                if (d > max)
                {
                    max = d;
                }
            }
            return max;
        }
        private static int DistanceToEdge(int[] coords, IMap map)
        {
            int min = int.MaxValue;
            for (int d = 0; d < coords.Length; d++)
            {
                int len = map.GetLength(d);
                int toEdge = Math.Min(coords[d], len - 1 - coords[d]);
                if ( toEdge < min)
                {
                    min = toEdge;
                }
            }
            return min;
        }
        private static bool IsOnBorder(int[] coords, IMap map, int thickness)
        {
            for (int d = 0; d < coords.Length; d++)
            {
                int len = map.GetLength(d);
                if (coords[d] < thickness || coords[d] >= len - thickness)
                {
                    return true; 
                }
            }
            return false;
        }
        private static int[] GetCenter(IMap map)
        {
            int dims = map.GetDimensionCount();
            int[] center = new int[dims];
            for (int d = 0; d < dims; d++)
            {
                center[d] = (map.GetLength(d) - 1) / 2;
            }
            return center;
        }
        private static void ValidateParameter<T>(IMap map, string parameterName, T value)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            if (string.IsNullOrEmpty(parameterName)) throw new ArgumentNullException(nameof(parameterName));
            var p = map.ParameterDefinitions.FirstOrDefault(x => x.Name == parameterName);
            if (p == null) throw new ArgumentException($"Parameter '{parameterName}' not defined.", nameof(parameterName));
            if (!p.IsValidObject(value)) throw new ArgumentException($"Invalid value for '{parameterName}'.", nameof(value));
        }
    }
}
