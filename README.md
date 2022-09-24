# QueryX
QueryX allows performing filtering, paging and sorting to IQueryable ends using URL query strings.

## Installation
Install with nuget:

```
Install-Package QueryX
```

## Usage
Initially it is necessary to add QueryX to the registered services. In ```Program.cs```:

```csharp
builder.Services.AddQueryX();
```

A filter model is needed to define the properties that will be used for filtering, this model could be an specific filter class, a Dto or a Domain model object.

```csharp
public class Card
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Priority { get; set; }
    public float EstimatedPoints { get; set; }
    public List<User> Owners { get; set; }
}
```
By default all properties can be used for filtering and sorting, this could be customized using [attributes](#customize-filter-model).

Two classes from QueryX are required for creating filters for the ```Card``` example object:

- **QueryBuilder,** this class should be injected in controllers or where the queries needs to be created.
- **QuerModel,** this class is used for capturing queries from URL query strings, it provides ```Filter```, ```OrderBy```, ```Offset``` and ```Limit``` properties for this purpose.

Additionally, an entity framework context is required for applying the queries:

```csharp
[HttpGet]
public IActionResult List([FromQuery] QueryModel queryModel)
{
    var query = _queryBuilder.CreateQuery<Card>(queryModel);
    var result = _context.Set<Card>().ApplyQuery(query).ToList();
    return Ok(result);
}
```

## Filtering
Filtering is made using operators, so a filter is defined this way: ```propertyName operator value```. 

It is possible to combine multiple filters using "and" (```&```) and "or" (```|```) connectors:
```
id>1 & title=-'test' | priority|=1,2
```
For facilitating writing queries in URL, ```;``` (semicolon) character can be used instead for representing the **and** connector:
```
id>1 ; title=-'test' | priority|=1,2
```
### Filter grouping
Filters can be also grouped using parentheses to determine how they should be evaluated:
```
id>1 ; (title=-'test' | priority|=1,2)
```

### Collection filters
It is possible to specify filters for collection properties with the following syntax:
```
propertyName(childPropertyName operator value)
```
The above code will use the ```Enumerable.Any``` method for applying the conditions. 

For using the ```Enumerable.All``` method:
```
propertyName*(childPropertyName operator value)
```
An example using the ```Card``` object would be:
```
owners(id==1 | name=='user2')
```
### Supported value types

- **Number,** integer, float, real, etc.
- **String,** this values should be wrapped in single quotes. Datetime and enum values needs to be specified as strings.
- **Booleans,** *true* and *false* keywords.
- **Null,** according the property type, *null* could be a valid value.

### Operators
|Operator    |Description                         |Comment|
|:----------:|------------------------------------|------------------|
| ==         |Equals operator                     | |
| ==*        |Case insensitive equals             |String type only |
| !=         |Not equals                          | |
| !=*        |Case insensitive not equals         |String type only |
| <          |Less than                           | |
| <=         |Less than or equals                 | |
| >          |Greater than                        | |
| >=         |Greater than or equals              | |
| -=-        |Contains                            |String type only |
| -=-*       |Case insensitive contains           |String type only |
| =-         |Starts with                         |String type only |
| =-*        |Case insensitive starts with        |String type only |
| -=         |Ends with                           |String type only |
| -=*        |Case insensitive ends with          |String type only |
| \|=        |In                                  |Allows multiple values |
| \|=*       |Case insensitive in                 |String type only. Allows multiple values |
| !\|=       |Not in                              |Allows multiple values |
| !\|=*      |Case insensitive not in             |String type only. Allows multiple values |

*Multiple values are specified this way:* ```val1,val2,val3```

## Customize filter model
## Custom filters
## Sorting and Paging
## Using different model for filtering
