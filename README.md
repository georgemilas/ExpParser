
This is a C# library that I have used (with similar implementations) for various projects, enabling power users to have very flexible query capabilities in the UI
  * One such example was a custom implementation of OData type queries in an API, as what OData supported out of the box was limited (today, I may go for GraphQL instead)
  * Other examples included a personal photo blog, testing on custom ORM implementation etc.
    
It supports arithmetic and boolean logic type queries, combined with regular expressions, in a single query.
 

Here are a couple of examples:
  * arithmetic and boolean 
      * (Age >= 12 and Gender = Male) or Gender != Male
      * Weight <= 300 and (Sedation = null or Sedation = true)
  * using eq, ne, gt, gte, lt etc. instead of =, !=, >, >=, < 
      * (Rate ne 0 and Type eq H) or Type ne H
  * fine-tuning SQL queries used internally
      * (tag echo) and not (jim fred)
      * <pre>
            new SQLTokenEvaluator("fld_name", SQLTokenEvaluator.OPERATOR_TYPE.LIKE, SQLTokenEvaluator.FIELD_TYPE.STRING);
        </pre> 
        "(((fld_name LIKE '%tag%') OR (fld_name LIKE '%echo%')) AND NOT (((fld_name LIKE '%jim%') OR (fld_name LIKE '%fred%'))))"



