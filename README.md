# DeployMe
Deze applicatie is ontworpen om een betrouwbaar en veilig voorbeeld te bieden. Het stelt ontwikkelaars in staat om hun lokaal gebouwde applicatie eenvoudig te hosten op gratis Azure-resources en bij te werken met behulp van GitHub als alternatief voor Azure Devops.

De applicatie is een eenvoudige .NET web API die gebruik maakt van een SQL database met behulp van Entity Framework inclusief migraties. 

Hieronder staat de werking beschreven. Hoe je je Azure omgeving en GitHub repository configureert lees je in deze [instructie](./SETUP.md).

## Werking
Er is een afzonderlijke build en deploy stap, en afhankelijk van de branch waarop code wordt gepushed worden de volgende workflows gestart.

| Branch    | Actie        |
|-----------|--------------|
| `main`-branch      |  [build-workflow](./.github/workflows/build.yml) & [deploy-workflow](./.github/workflows/deploy.yml)  |
| overige branches      | [build-workflow](./.github/workflows/build.yml)  |

### Build-workflow
Wanneer deze succesvol is afgerond (inclusief tests) zijn er artifacts gepubliceerd die gebruikt kunnen worden in de deploy-workflow. 

De artifacten bestaan uit:
1. Een release-versie van de applicatie
2. Een migratie-executable om het database-schema bij te werken.

### Deploy-workflow
Deze workflow triggert automatisch de build-workflow en gebruikt de gegenereerde artifacts om zowel de Azure web-app als de Azure SQL-database bij te werken. Daarnaast configureert de workflow de verbinding tussen je Azure web-applicatie en de Azure SQL-database, zodat je dit niet handmatig hoeft te doen.

## Lokaal ontwikkelen
De applicatie maakt momenteel gebruik van één secret: `ConnectionStringAzureSQL`

Omdat we secrets nooit in de broncode opslaan (en liever ook niet in de buurt) kun je gebruik maken van `dotnet user-secrets`. Een [veilige manier om lokaal secrets op te slaan](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows) zonder de appsettings.json te gebruiken. 

Open een command line en navigeer naar de map met daarin het `DeployMe.API.csproj` bestand. Voer het volgende commando uit

`dotnet user-secrets set "ConnectionStringAzureSQL" "<jouw_lokale_connectionstring_hier>"`.

Je kunt nu de applicatie ontwikkelen met behulp van een lokale database. Om een EF Core migratie aan te maken gebruik je het volgende commando:

`dotnet ef migrations add InitialCreate --startup-project DeployMe.API --project DeployMe.Infrastructure`

Om je lokale database bij te werken gebruik je

`dotnet ef database update --startup-project DeployMe.API --project DeployMe.Infrastructure`

### SQL schema voor Entity Framework Core 
Deze applicatie maakt standaard tabellen aan in een SQL-schema genaamd `DeployMe`. De naam van dit schema vind je in het EF-contextbestand, en je kunt deze aanpassen zolang er nog geen migraties zijn uitgevoerd.

Met een schema kun je meerdere EF Core-applicaties gescheiden laten werken binnen één enkele database. Hoewel dit normaal gesproken niet als best practice wordt beschouwd, is deze configuratie toch opgenomen omdat Azure slechts één gratis database biedt. Op deze manier heb je meer flexibiliteit.

Lees meer over SQL-schema's en hoe je deze veilig kunt gebruiken voor gescheiden applicaties (en SQL-gebruikers) [hier](https://www.sqlshack.com/a-walkthrough-of-sql-schema/).

## Azure & GitHub configureren
Lees de [instructie](./SETUP.md) hoe je jouw Azure Resources en GitHub repository configureert.
