namespace ResourceInformationV2.Data.DataHelpers {

    public static class ListExtentions {

        public static List<T> MoveItemDown<T>(this List<T> list, T item) {
            ArgumentNullException.ThrowIfNull(list);
            var index = list.IndexOf(item);
            if (index < 0 || index >= list.Count - 1) {
                return list;
            }
            var temp = list[index + 1];
            list[index + 1] = item;
            list[index] = temp;
            return list;
        }

        public static List<T> MoveItemUp<T>(this List<T> list, T item) {
            ArgumentNullException.ThrowIfNull(list);
            var index = list.IndexOf(item);
            if (index <= 0) {
                return list;
            }
            var temp = list[index - 1];
            list[index - 1] = item;
            list[index] = temp;
            return list;
        }
    }
}