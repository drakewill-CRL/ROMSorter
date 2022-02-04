# ROMSorter
Detect and rename ROMs and other games.
Requires .NET 6. current release for ROMSorteris Win-x64 only. Librarian is available for all platforms.

Main app:

![image](https://user-images.githubusercontent.com/46307022/152562490-ed44a368-0ea3-43b5-84dd-0c22e2330951.png)


* Zip (or convert) all files: Zip each file in the folder to a new zip file. Zip files will be re-zipped for consistency. Non-zip archive files (rar, gz/gzip, tar, 7z) will be extracted and then zipped. Should handle files of any size, in case you're working on ISO images or something.

* Detect Duplicate Files: If a file in the folder is identical to another one, it moves the duplicates to a sub-folder (Duplicates/[OriginalFileName]/) so you can identify which file they're a duplicate of. Works best on unzipped files, since identical files with different names inside a zip file will create non-identical zip files.

* Unzip all files: The opposite button. Extracts all files from all archives in the current folder.

* Catalog Files: Saves a small file with the name and hashes of each file in the folder. Intended for backups, particularly optical media, so you can quickly check in the future if files have become corrupted without having to attempt to read or play each manually.

* Verify Catalog: Reads the saves catalog file, and confirms if each file is still identical to when it was cataloged.

* Make DAT File for Folder: Similar to the catalog process, except the end result is a .DAT file that can be read by other ROM manager apps. 

* Rename Single-File games: If you provided a .dat file, this will check each file to see if it's present, and if so will rename it to match. Works best with TOSEC dats. No-Intro dats sometimes exclude headers and that requires additional work per file extension to detect and fix. The checkbox on the main screen determines if unidentified files are skipped or moved to their own sub-folder.

* Create CHD Files: Convert all the BIN/CUE or ISO files in a folder to CHD format. Requires chdman to be in the same folder as the main application, included with the Windows apps. Linux users will need to supply their own chdman executable

* Extract CHD Files: Converts CHD files back to BIN/CUE format. Requires chdman to be in the same folder as the main application, included with the Windows apps. Linux users will need to supply their own chdman executable

* 1G1R Sort: Make a 1 Game, 1 ROM (1G1R) set based on a Parent/Clone DAT. Lets you pick priority on which regions should be taken when multiple ones are present. Pops up a list of regions available and asks you to set the order for them. Will inform you if your DAT is not the correct format for this to succeed. 

* Everdrive Sort: For hardware devices like the Everdrive with a low cap on maximum files per folder. Makes a sub-folder for each letter, and moves files into the appropriate folder. Unzip and rename files first for best results.
