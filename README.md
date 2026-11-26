# Item Finder

Item Finder lists the items in `Data.txt` alphabetically and prints the directions to the item you choose.

## How the search works

The program reads `Data.txt` into memory and checks the tree from top to bottom. It saves each item's original line number so it can find the item again after sorting the menu. If it finds a malformed entry, a duplicate item name, or a missing parent direction, it reports the line number and stops.

Each run looks up one item, so the program traces only that item's route. Starting at its saved line, it scans upward for the nearest direction one indentation level to the left. It repeats that step until it reaches the root, then reverses the directions into walking order.

## Build and run

Install the .NET 10 SDK. Open a terminal in the folder containing `ItemFinder.csproj` and `Data.txt`, then run these commands.

```sh
dotnet build
dotnet run
```

Enter an item number and press Enter. With the included `Data.txt`, choose `2` for `Envelope Pack`. The last step should be `Check the filing cabinet.`

## Personal note

I designed and wrote the program, and I used AI to format the code and write the comments and documentation.

