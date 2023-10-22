# QueryX
QueryX allows performing filtering, paging and sorting to IQueryable ends using URL query strings.

## Installation
Install with nuget:

```
Install-Package QueryX
```

## Usage
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
By default all properties can be used for filtering and sorting, this could be customized using [fluent configuration](#customize-filter-model).

### Example

QueryX adds ```ApplyQuery``` extension methods to IQueryable interface to perform queries:

```csharp
var filter = "priority > 1";
var result = _context.Set<Card>().ApplyQuery(filter).ToList();
```

## Filtering
Filtering is made using operators, so a filter is defined this way: ```propertyName operator value```. 

It is possible to combine multiple filters using "and" (```&```) and "or" (```|```) logical operators:
```
id>1 & title=-'test' | priority|=1,2
```
For facilitating writing queries in URL, ```;``` (semicolon) character can be used instead for representing the **and** logical operator:
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
|Category            |Description                         |
|--------------------|------------------------------------|
| Numbers            |integer, float, real, etc.          |
| String, Char       |should be wrapped in single quotes  |
| DateTime, Timespan |should be wrapped in single quotes  |
| Enums              |should be wrapped in single quotes  |
| Constants          |true, false, null                   |

### Operators
|Operator    |Description                         |Comment|
|:----------:|------------------------------------|------------------|
| ==         |Equals operator                     | |
| !=         |Not equals                          | |
| <          |Less than                           | |
| <=         |Less than or equals                 | |
| >          |Greater than                        | |
| >=         |Greater than or equals              | |
| -=-        |Contains                            |String type only |
| =-         |Starts with                         |String type only |
| -=         |Ends with                           |String type only |
| \|=        |In                                  |Allows multiple values |

*Multiple values are specified this way:* ```0,1,2``` *or* ```'val1','val2','val3'``` *if the values are strings*

### Case insensitive operator
All operators can be combined with the case insensitive operator (```*```) for ignoring case when comparing strings:

```
title ==* 'TeSt VaLuE'
```

This operator is intended to work only with string properties

### Not Operator
The not operator (```!```) can be applied to any filter, collection filter or group:

```
!id>1 ; !title=-'test' | !priority|=1,2
```

```
id>1 ; !(title=-'test' | priority|=1,2)
```

```
!owners(id==1 | name=='user2')
```

## Sorting and Paging
The ```ApplyOrderingAndPaging``` method allow specifying ordering.

For ascending order base on property ```Title```:
```
title
```

For descending order:
```
-title
```

It is possible to combine multiple orderings:
```
id,-priority,title
```

Example:
```csharp
var sortBy = "-estimatedPoints";
var offset = 10;
var limit = 10;
_context.Set<Card>().ApplyOrderingAndPaging(sortBy, offset, limit);
```

## Customize filtering and sorting

It is possible to customize the filtering behavior with ```QueryMappingConfig``` class:

```csharp
QueryMappingConfig.Global
    .For<Card>(cfg => 
    {
        // mapping configurations
    });
```

#### Map property name:
Allows mapping a property with a different name that will appear in the ```filter``` or ```sortBy``` string
```csharp
cfg.Property(c => c.Priority).MapFrom("queryPriority");
```

#### Ignore properties:
Ignore properties from filter and sorting steps. ```IgnoreFilter``` and ```IgnoreSort``` methods exists also.
```csharp
cfg.Property(c => c.Priority).Ignore();
```



#### Custom filters:
This propertiess are excluded as part of the filter, custom code needs to be written for doing something with the filter values. Custom filters are applied after all filters have been applied
```csharp
cfg.Property(c => c.Priority).CustomFilter((source, values, op) => 
{
    // source   : IQueryable instance to apply the custom filter
    // values   : values specified in filter string
    // op       : operator for this filter
    return source.Where(c => c.Priority == values[0]);
});
```

#### Custom sort:
This propertiess are excluded as part of the sort behavior, custom code needs to be written to apply them. Custom filters are applied after all filters have been applied
```csharp
cfg.Property(c => c.Priority).CustomSort((source, ascending, isOrdered) => 
{
    // source   : IQueryable instance to apply the custom filter
    // ascending: indicates if the sorting should be done in ascending direction
    // isOrdered: indicates if the IQueryable instace have been already ordered, if true, ThenBy and ThenByDescending methods should be used as appropriate
    return isOrdered
        ? ascending
            ? source.ThenBy(c => c.Priority)
            : source.ThenByDescending(c => c.Priority)
        : ascending
            ? source.OrderBy(c => c.Priority)
            : source.OrderByDescending(c => c.Priority);
});
```

### Override global configurations
The configurations created using ```QueryMappingConfig.Global``` are applied globally. ```ApplyQuery``` and ```ApplyOrderingAndPaging``` methods accepts a ```QueryMappingConfig``` instance in case specific configuration is needed:
```csharp
var config = new QueryMappingConfig();
config.For<Card>(cfg =>
{
    // set specific configurations
});

var filter = "id |= 2,4,6";
_context.Set<Card>().ApplyQuery(filter, mappingConfig: config);
```

Extending global configuration is also possible:

```csharp
var config = QueryMappingConfig.Global.Clone()
config.For<Card>(cfg =>
{
    // set specific configurations
});

var filter = "id |= 2,4,6";
_context.Set<Card>().ApplyQuery(filter, mappingConfig: config);
```

## QueryModel
```QueryModel``` is a class that can be used to capture user parameters in a WebAPI endpoint, it contains ```Filter```, ```SortBy```, ```Offset``` and ```Limit``` properties. ```ApplyQuery``` and ```ApplyOrderingAndPaging``` methods have overloads to receive a ```QueryModel``` instance.

## Query exceptions
By default invalid properties will be ignored for filtering and ordering but it is possible to  change this behavior by calling ```ThrowQueryExceptions()``` when registering QueryX:

```csharp
builder.Services.AddQueryX(o => o.ThrowQueryExceptions());
```

These exceptions will be thrown as appropriate:
* ```InvalidFilterPropertyException``` for filtering errors
* ```InvalidOrderingPropertyException``` for ordering errors

