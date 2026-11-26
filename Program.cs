using System;
using System.IO;
using System.Collections.Generic;

namespace ItemFinder
{
    public class Program
    {
        private const int IndentWidth = 3;
        private const string ItemPrefix = " Item: ";

        public static void Main(string[] args)
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines("Data.txt");
            }
            catch (IOException)
            {
                Console.Error.WriteLine("Could not read Data.txt. Check that it exists in the current folder.");
                return;
            }

            var itemLines = new Dictionary<string, int>();

            // The root starts in column zero, so its calculated parent column is -IndentWidth.
            int deepestDirectionColumn = -IndentWidth;

            // Check the tree from top to bottom so each direction is processed before its children.
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex].TrimEnd();
                int column = GetPrefixLength(line);
                string entry = line.Substring(column);
                bool isDirection = entry.StartsWith("+ ", StringComparison.Ordinal);

                if (column > 0 && entry.StartsWith(ItemPrefix, StringComparison.Ordinal))
                {
                    string name = entry.Substring(ItemPrefix.Length).Trim();
                    if (!itemLines.TryAdd(name, lineIndex))
                    {
                        Console.Error.WriteLine($"Duplicate item '{name}' on line {lineIndex + 1}.");
                        return;
                    }
                }
                else if (!isDirection)
                {
                    Console.Error.WriteLine($"Invalid tree entry on line {lineIndex + 1}.");
                    return;
                }

                // Each non-root entry needs a parent direction one level above it.
                int parentColumn = column - IndentWidth;

                if (parentColumn > deepestDirectionColumn)
                {
                    Console.Error.WriteLine($"Line {lineIndex + 1} has no parent direction at this level.");
                    return;
                }

                // An item has no children, so keep its parent as the deepest direction.
                deepestDirectionColumn = isDirection ? column : parentColumn;
            }

            if (itemLines.Count == 0)
            {
                Console.Error.WriteLine("Data.txt contains no items.");
                return;
            }

            var itemNames = new List<string>(itemLines.Keys);
            itemNames.Sort(StringComparer.OrdinalIgnoreCase);
            Console.WriteLine("Available items:");

            for (int i = 0; i < itemNames.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] - {itemNames[i]}");
            }

            Console.Write("What item would you like to search for? ");

            if (!int.TryParse(Console.ReadLine(), out int selection) ||
                selection < 1 || selection > itemNames.Count)
            {
                Console.Error.WriteLine($"Please enter a number from 1 to {itemNames.Count}.");
                return;
            }

            string selectedItem = itemNames[selection - 1];
            int itemLineIndex = itemLines[selectedItem];
            
            List<string> directions = FindDirections(lines, itemLineIndex);

            foreach (string direction in directions)
            {
                Console.WriteLine(direction);
            }
        }

        /// <summary>
        /// Returns the steps from the starting point to the item at the given zero-based line index.
        /// </summary>
        private static List<string> FindDirections(string[] lines, int itemLineIndex)
        {
            var directions = new List<string>();
            int parentColumn = GetPrefixLength(lines[itemLineIndex]) - IndentWidth;

            // Search upward for the nearest direction at each parent column.
            for (int i = itemLineIndex - 1; i >= 0 && parentColumn >= 0; i--)
            {
                string line = lines[i];

                if (GetPrefixLength(line) == parentColumn && line[parentColumn] == '+')
                {
                    // Skip the "+ " marker when copying the direction text.
                    directions.Add(line.Substring(parentColumn + 2));
                    parentColumn -= IndentWidth;
                }
            }

            directions.Reverse();
            return directions;
        }

        /// <summary>
        /// Measures the tree guides and branch at the start of a data line.
        /// </summary>
        /// <returns>The index immediately after the branch, or zero for a root line or an invalid prefix.</returns>
        private static int GetPrefixLength(string line)
        {
            for (int column = 0; column <= line.Length - IndentWidth; column += IndentWidth)
            {
                char marker = line[column];
                bool isBranch = marker == '├' || marker == '└';
                bool isGuide = marker == ' ' || marker == '|' || marker == '│';

                if (!isBranch && !isGuide)
                {
                    return 0;
                }

                // A branch is filled with horizontal lines and a guide is filled with spaces.
                char expectedFill = isBranch ? '─' : ' ';

                for (int offset = 1; offset < IndentWidth; offset++)
                {
                    if (line[column + offset] != expectedFill)
                    {
                        return 0;
                    }
                }

                if (isBranch)
                {
                    return column + IndentWidth;
                }
            }

            return 0;
        }
    }
}
