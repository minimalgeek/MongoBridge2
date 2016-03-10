# MongoDB to AmiBroker Bridge

A very basic implementation to support data exchange between MongoDB and AmiBroker.
.NET for AmiBroker plugin is required!

Example:

```
TraceTest();

Connect("mongodb://localhost", "databaseName", "collectionName");
MongoQueryToAFL("indexName=" + Name(), 
				"publishDate", 
				"aCol;bCol;cCol.dNested;eCol.fNested");

AddColumn(aCol, "A Column");
AddColumn(bCol, "B Column");
AddColumn(cCol_dNested, "C Column's D Nested Field");
AddColumn(eCol_fNested, "E Column's F Nested Field");

Filter = 1;
```