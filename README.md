# DBPFSharp

A C# library that reads and writes SimCity 4 DBPF files.

## Exsmple applications

The following example application demonstrate how to use the library.
The applications are located in the _examples_ folder.

### DPBFcreate

This application crates a new DBPF file and inserts the specified item (optionally compressing it).

### DBPFextract

This application extracts the specified item from a DBPF file.
If no item is specified it will extract the first item in the DBPF file.

The extracted item will be decompressed if necessary.

## License

This project is licensed under the terms of the MIT License.   
See [License.txt](License.txt) for more information.