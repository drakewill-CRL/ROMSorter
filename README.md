# ROMSorter
Detect and rename ROMs and other games.
Requires .NET 6, current release (1) is Win-x64 only. Source code and console apps should run on nearly anything.

Main app:

![screenshot1](https://user-images.githubusercontent.com/46307022/146663715-1d764956-331e-4852-8f50-c82cd984bde9.png)

* Zip (or convert) all files: Zip each file in the folder to a new zip file. Zip files will be re-zipped for consistency. Non-zip archive files (rar, gz/gzip, tar, 7z) will be extracted and then zipped. Should handle files of any size, in case you're working on ISO images or something.

* Unzip all files: The opposite button. Extracts all files from all archives in the current folder.

* Rename Single-File games: If you provided a .dat file, this will check each file to see if it's present, and if so will rename it to match. Works best with TOSEC dats. No-Intro dats sometimes exclude headers and that requires additional work per file extension to detect and fix. The checkbox on the main screen determines if unidentified files are skipped or moved to their own sub-folder.

* Detect Duplicate Files: If a file in the folder is identical to another one, it moves the duplicates to a sub-folder (Duplicates/[OriginalFileName]/) so you can identify which file they're a duplicate of. Works best on unzipped files, since identical files with different names inside a zip file will create non-identical zip files.

* Catalog Files: Saves a small file with the name and hashes of each file in the folder. Intended for backups, particularly optical media, so you can quickly check in the future if files have become corrupted without having to attempt to read or play each manually.

* Verify Catalog: Reads the saves catalog file, and confirms if each file is still identical to when it was cataloged.

