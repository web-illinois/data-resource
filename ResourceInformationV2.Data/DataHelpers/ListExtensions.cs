namespace ResourceInformationV2.Data.DataHelpers {

    public static class ListExtensions {
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
        public static List<T> MoveItemToBottom<T>(this List<T> list, T item) {
            ArgumentNullException.ThrowIfNull(list);
            var index = list.IndexOf(item);
            if (index < 0 || index >= list.Count - 1) {
                return list;
            }
            for (var i = index; i < list.Count - 2; i++) {
                list[i] = list[i + 1];
            }
            list[^1] = item;
            return list;
        }
        public static List<T> MoveItemToTop<T>(this List<T> list, T item) {
            ArgumentNullException.ThrowIfNull(list);
            var index = list.IndexOf(item);
            if (index <= 0) {
                return list;
            }
            for (var i = index - 1; i > 0; i--) {
                list[i + 1] = list[i];
            }
            list[0] = item;
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