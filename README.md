# DeployMe
Deze applicatie is ontworpen om een betrouwbaar en veilig voorbeeld te bieden. Het stelt ontwikkelaars in staat om hun lokaal gebouwde applicatie eenvoudig te hosten op gratis Azure-resources en bij te werken met behulp van GitHub als alternatief voor Azure Devops.

De applicatie is een eenvoudige .NET web API die gebruik maakt van een SQL database met behulp van Entity Framework inclusief migraties. 

Hieronder staat de werking beschreven. Hoe je je Azure omgeving en GitHub repository configureert lees je in deze [instructie](./SETUP.md).

## Werking
Er is een afzondwerlijke build en deploy stap, en afhankelijk van de branch waarop code wordt gepushed worden de volgende workflows gestart.

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
Deze workflow start zelf de build-workflow en gebruikt de artifacts uit deze workflow om de Azure web app en Azure SQL database bij te werken. Deze workflow configureert ook de verbinding tussen je Azure web applicatie en Azure SQL database zodat je dit zelf niet hoeft te doen. 

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
Deze applicatie maakt standaard de tabellen aan in een SQL schema genaamd `DeployMe`. Je vindt de naam in het EF-context bestand en kunt deze aanpassen wanneer je nog geen migraties hebt uitgevoerd.

Met een schema kun je meerdere EF core applicaties afzonderlijk gebruik laten maken van één enkele database. Een de praktijk is dit geen best practise, maar aangezien je maar één gratis database van Azure kunt gebruiken is deze configuratie toch opgenomen.

Lees meer over SQL schemas en hoe je deze veilig kunt gebruiken met afzonderlijke applicaties (en SQL users) [hier](https://www.sqlshack.com/a-walkthrough-of-sql-schema/).

## Azure & GitHub configureren
Lees de [instructie](./SETUP.md) hoe je jouw Azure Resources en GitHub repository configureert.