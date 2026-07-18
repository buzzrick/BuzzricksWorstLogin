using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Runaway.Extensions
{
	public static class ListExtensions
	{
        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return true;

            // Efficiency gain from following cast is only felt when working
            // with SQL - in most cases this is a performance hit.

            //ICollection<T> collection = enumerable as ICollection<T>;
            //if (collection != null)
                //return collection.Count < 1;

            return !enumerable.Any();
        }

        /// <summary>
        /// Removes and returns a random item from the list
        /// </summary>
        public static T PopRandom<T>(this IList<T> list) where T : class
        {
            if (list.Count == 0)
            {
                return null;
            }
            int index = UnityEngine.Random.Range(0, list.Count);
            T element = list[index];
            list.RemoveAt(index);
            return element;
        }

        /// <summary>
        /// Removes and returns a random item from the list. Throws <see cref="InvalidOperationException"/>
        /// if the list is empty.
        /// </summary>
        public static T PopRandomStruct<T>(this IList<T> list) where T : struct
        {
            if (list.Count == 0)
            {
                throw new InvalidOperationException("List is empty.");
            }
            int index = UnityEngine.Random.Range(0, list.Count);
            T element = list[index];
            list.RemoveAt(index);
            return element;
        }

        /// <summary>
        /// Removes and returns the first element in the list that matches the
        /// given condition, or the default value for the lists' type if nothing
        /// matches the condition.
        /// </summary>
        public static T PopFirstOrDefault<T>(this IList<T> list, Predicate<T> predicate)
        {
            // Iterate list looking for an element that matches given condition
            int index = -1;
            for (int i = 0; i < list.Count && index < 0; i++)
            {
                if (predicate(list[i]))
                    index = i;
            }

            // If thing was found in list remove and return it
            if (index > -1)
                return list.Pop(index);

            // Otherwise return default (likely null)
            return default;
        }

        /// <summary>
        /// Removes and returns the element at the given index in the list
        /// </summary>
        public static T Pop<T>(this IList<T> list, int index)
        {
            T element = list[index];
            list.RemoveAt(index);
            return element;
        }
        /// <summary>
        /// Returns a shuffled copy of the input. Does not modify the unput.
        /// </summary>
        public static List<T> Shuffle<T>(this IList<T> deckList)
		{
			var deck = deckList.ToArray();

			for (int i = 0; i < deck.Length; i++)
			{
				T temp = deck[i];
				int randomIndex = UnityEngine.Random.Range(0, deck.Length);
				deck[i] = deck[randomIndex];
				deck[randomIndex] = temp;
			}
			return deck.ToList<T>();
		}

		public static T RandomItem<T>(this IList<T> itemList)
		{
			if (itemList.Count == 0)
				return default(T);
			int randomIndex = UnityEngine.Random.Range(0, itemList.Count);
			return itemList[randomIndex];
		}

        public static T RandomItem<T>(this IEnumerable<T> itemList)
        {
            return itemList.ToList<T>().RandomItem<T>();
        }


            /// <summary>
            /// Gets a number of unique random items from the list. Rather inefficient when
            /// dealing with large lists.
            /// </summary>
        public static IEnumerable<T> UniqueRandomItems<T>(this IList<T> itemList, int count)
        {
            // Only pick items if there is something in the list
            if (itemList.Count > 0)
            {

                // Asked for as many or more item than there are in the list? - just
                // return the whole list.
                if (count >= itemList.Count)
                {
                    foreach (T item in itemList)
                        yield return item;
                }

                // Remember the indices we've picked already
                List<int> pickedIndices = new List<int>();

                for (int i = 0; i < count; i++)
                {
                    // HACK: this is pretty bad to do when dealing with large lists.

                    // Randomly get indicies until we've got one we haven't picked
                    // before.
                    int index = UnityEngine.Random.Range(0, itemList.Count);
                    while (pickedIndices.Contains(index))
                        index = UnityEngine.Random.Range(0, itemList.Count);

                    yield return itemList[index];
                    pickedIndices.Add(index);
                }
            }
        }

        public static IEnumerable<T> UniqueRandomItems<T>(this IEnumerable<T> itemList, int count)
        {
            return itemList.ToList<T>().UniqueRandomItems<T>(count);
        }

        /// <summary>
        /// Flattens the collection into a string containing the string
        /// representation of each element in the collection separated by the
        /// given separator
        /// </summary>
        public static string ToString(this IEnumerable collection, string separator)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in collection)
            {
                builder.Append(item.ToString());
                builder.Append(separator);
            }

            // Remove the last separator
            builder.Remove(builder.Length - separator.Length, separator.Length);

            return builder.ToString();
        }
	}
}
