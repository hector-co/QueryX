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
- **QueryModel,** this class is used for capturing queries from URL query strings, it provides ```Filter```, ```OrderBy```, ```Offset``` and ```Limit``` properties for this purpose.

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

## Customize filter model
By default all properties from a filter model can be used for filtering and ordering, but there are some attributes that allows to have control on this

Properties marked wth the ```QueryIgnoreAttribute``` attribute will be ignored for filtering and ordering:
```csharp
[QueryIgnore]
public float EstimatedPoints { get; set; }
```

Also, with ```QueryOptionsAttribute``` attribute some other options could be specified:
```csharp
[QueryOptions(Operator = OperatorType.Equals, IsSortable = false, 
    ParamsPropertyName = "EstimatedPts", ModelPropertyName = "Estimation")]
public float EstimatedPoints { get; set; }
```
* ```Operator``` will set the default operator for this property, ignoring the one sent in the query string
* ```IsSortable``` determines if this property is sortable or not, true by default
* ```ParamsPropertyName``` is used for mapping filter names from query string. In this case, in query string will be a filter named ```EstimatedPts``` that will be mapped to the ```EstimatedPoints``` property
* ```ModelPropertyName``` can be used when the filter model is different than the entity in DbContext and the filter model and the entity model have different property names, especifically, this allows mapping this property to a different one in the entity model. In this example, the value for this property will be used in the ```Estimation``` property in the entity model because the ```IQueryable``` filter needs to be created using the entity model properties

## Sorting and Paging
The ```OrderBy``` property in ```QueryModel``` object allows specifying ordering.

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

Custom filters can not be used for ordering

## Custom filters
The ```CustomFilterAttribute``` attribute allows changing the default behavior of filters:

```csharp
[CustomFilter]
public int Priority { get; set; }
```
Properties marked with this attribute will be excluded as part of the filter, custom code needs to be written for doing something with the filter value after the ```Query``` object is created:

```csharp
[HttpGet]
public IActionResult List([FromQuery] QueryModel queryModel)
{
    var query = _queryBuilder.CreateQuery<Card>(queryModel);
    var queryable = _context.Set<Card>();

    // Applying custom filter
    if (query.TryGetFilter(m => m.Priority, out var filter))
    {
        var filterValue = filter.Values.First();
        queryable = queryable.Where(m => m.Priority == filterValue);
    }

    var result = queryable.ApplyQuery(query).ToList();
    return Ok(result);
}
```
Custom filters are also ignored in the sorting process but it is possible to know if they were passed as part of the sort string:

```csharp
[HttpGet]
public IActionResult List([FromQuery] QueryModel queryModel)
{
    var query = _queryBuilder.CreateQuery<Card>(queryModel);
    var queryable = _context.Set<Card>();

    // Get custom order by
    if (query.TryGetOrderBy(m => m.Priority, out var orderBy))
    {
        // apply order by manually using orderBy.Ascending and orderBy.PropertyName as needed
    }

    var result = queryable.ApplyQuery(query).ToList();
    return Ok(result);
}
```

## Query exceptions
By default invalid properties will be ignored for filtering and ordering but it is possible to  change this behavior by setting ```ThrowQueryExceptions``` to true when registering QueryX:

```csharp
builder.Services.AddQueryX(o => o.ThrowQueryExceptions = true);
```

These exceptions will be thrown as appropriate:
* ```InvalidFilterPropertyException``` for filtering errors
* ```InvalidOrderingPropertyException``` for ordering errors

