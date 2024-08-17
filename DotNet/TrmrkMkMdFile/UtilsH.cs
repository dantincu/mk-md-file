using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TrmrkMkMdFile
{
    /// <summary>
    /// Utility class for helper methods.
    /// </summary>
    public static class UtilsH
    {
        /// <summary>
        /// The path where the executing assembly is located. That is the path where the <c>.exe</c> file and <c>.dll</c> files,
        /// along with the <c>.json</c> config file for this program reside.
        /// </summary>
        public static readonly string ExecutingAssemmblyPath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Searches the provided enumerable for and returns the first item that satisfies the condition specified by the provided
        /// predicate or a default value of no suitable item is found.
        /// </summary>
        /// <typeparam name="T">The type of items that the provided enumerable contains.</typeparam>
        /// <param name="nmrbl">The enumerable containing the items to search in.</param>
        /// <param name="predicate">The predicate specifying the condition that the suitable item must satisfy.</param>
        /// <returns>A value of type <see cref="KeyValuePair{int, T}" /> containing the index of the first item
        /// that satisfies the condition specified by the predicate and the value of the actual item, if one
        /// has been found. If no suitable item has been found, it returns the value <c>null</c> for the item and
        /// <c>-1</c> for the item index.</returns>
        public static KeyValuePair<int, T> FirstKvp<T>(
            this IEnumerable<T> nmrbl,
            Func<T, int, bool> predicate)
        {
            KeyValuePair<int, T> retKvp = new KeyValuePair<int, T>(-1, default);
            int idx = 0;

            foreach (T item in nmrbl)
            {
                if (predicate(item, idx))
                {
                    retKvp = new KeyValuePair<int, T>(idx, item);
                    break;
                }
                else
                {
                    idx++;
                }
            }

            return retKvp;
        }

        /// <summary>
        /// Searches the provided enumerable for and returns the first item that satisfies the condition specified by the provided
        /// predicate or a default value of no suitable item is found.
        /// </summary>
        /// <typeparam name="T">The type of items that the provided enumerable contains.</typeparam>
        /// <param name="nmrbl">The enumerable containing the items to search in.</param>
        /// <param name="predicate">The predicate specifying the condition that the suitable item must satisfy.</param>
        /// <returns>A value of type <see cref="KeyValuePair{int, T}" /> containing the index of the first item
        /// that satisfies the condition specified by the predicate and the value of the actual item, if one
        /// has been found. If no suitable item has been found, it returns the value <c>null</c> for the item and
        /// <c>-1</c> for the item index.</returns>
        public static KeyValuePair<int, T> FirstKvp<T>(
            this IEnumerable<T> nmrbl,
            Func<T, bool> predicate) => nmrbl.FirstKvp(
                (item, idx) => predicate(item));

        /// <summary>
        /// Takes the input value, converts it to an output value using the provided output value factory and returns the output value.
        /// It's a convenience method for using the return value of a method or expression without explicitly assigning its
        /// value to a local variable in the parent scope. Results in shorter and more concise code,
        /// but overuse might lead to ninja-code.
        /// </summary>
        /// <typeparam name="TIn">The input value type</typeparam>
        /// <typeparam name="TOut">The output value type.</typeparam>
        /// <param name="inVal">The provided input value to be passed in to the output value factory.</param>
        /// <param name="outValFactory">The provided output value delegate.</param>
        /// <returns>The value returned by the provided output value factory delegate.</returns>
        public static TOut With<TIn, TOut>(
            this TIn inVal,
            Func<TIn, TOut> outValFactory) => outValFactory(inVal);

        /// <summary>
        /// Returns the provided input string if it is not null or empty or the <c>null</c> value otherwise.
        /// </summary>
        /// <param name="str">The input string</param>
        /// <returns>The provided input string if it is not null or empty or the <c>null</c> value otherwise.</returns>
        public static string? Nullify(
            this string? str) => string.IsNullOrEmpty(
                str) ? null : str;

        /// <summary>
        /// Returns the provided input value boxed into a nullable int if the input value is different than <c>0</c>
        /// or the <c>null</c> value if the input value is equal to <c>0</c>.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <returns>The provided input value boxed into a nullable int if the input value is different than <c>0</c>
        /// or the <c>null</c> value if the input value is equal to <c>0</c>.</returns>
        public static int? Nullify(
            this int value)
        {
            int? retVal = null;

            if (value != 0)
            {
                retVal = value;
            }

            return retVal;
        }

        /// <summary>
        /// Extension method that calls the <see cref="string.Join(string?, string?[])" /> method.
        /// Because it is an extension method, it is easier to call than the CLR provided method.
        /// </summary>
        /// <param name="strArr">An array containing the string values to join</param>
        /// <param name="joinStr">A string to be used as a join parameter (i.e. inserted between each 2 adjacent strings).</param>
        /// <returns>The joined string.</returns>
        public static string JoinStr(
            this string[] strArr,
            string joinStr = null) => string.Join(
                joinStr ?? string.Empty, strArr);

        /// <summary>
        /// If the provided path is not null, it launches a Windows Explorer Process with this path
        /// as argument. If the provided path points to an existing file, this causes the default program for that file
        /// extension to open the file.
        /// </summary>
        /// <param name="path"></param>
        public static void OpenWithDefaultProgramIfNotNull(string path)
        {
            if (path != null)
            {
                using Process fileopener = new Process();

                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "\"" + path + "\"";
                fileopener.Start();
            }
        }

        /// <summary>
        /// Executes the provided delegate by wrapping its execution in a try/catch block. If executing the delegate
        /// results in an exception being thrown, the exception is being printed to the command prompt before that
        /// program exists.
        /// </summary>
        /// <param name="program"></param>
        public static void ExecuteProgram(
            Action program)
        {
            try
            {
                program();

                Console.ResetColor();
            }
            catch (Exception exc)
            {
                Console.WriteLine();
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;

                Console.WriteLine("AN UNHANDLED EXCEPTION WAS THROWN: ");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine();
                Console.WriteLine(exc);
                Console.ResetColor();
            }
        }
    }
}
