# DeployMe 
Een voorbeeld applicatie API die geschikt is voor gebruik op de **gratis** Azure App service & Azure SQL database en gebruik maakt van CI/CD d.m.v. GitHub Actions (inclusief SQL schema migraties).

Deze applicatie maakt gebruik van
* .NET Core 8
* Union architectuur
* Entity Framework (EF Core)
* GitHub build & deploy workflows

Om deze applicatie te ontwikkelen & deployen heb je nodig
* Een Azure (for Students) account
* Visual Studio (Code)
* SQL Server Management Studio

## Stap 1 - Aanmaken Azure Resources

### Een gratis database aanmaken
Azure biedt een levenslange gratis SQL database met uiteraard limieten. Wanneer je deze wilt activeren ga je naar: https://portal.azure.com/#create/Microsoft.SQLDatabase

Let bij het aanmaken van deze database op:
* Dat je de 'apply offer (preview)' activeert
* Een zone die ondersteuning biedt (bv. North Europe)
* Authentication method: Use both SQL and Microsoft Entra authentication
* Security: allow all Azure services, add Client IP
* Stel jouw account in als Entra administrator
* Noteer je SQL administrator username & password (niet vergeten)


Meer informatie over de gratis SQL database op: 
https://learn.microsoft.com/en-us/azure/azure-sql/database/free-offer?view=azuresql

### Een Azure free app service aanmaken
Azure biedt ook een gratis App Service met limieten. Deze kun je activeren door naar https://portal.azure.com/#create/Microsoft.WebSite te gaan

Let bij het aanmaken van deze app service op:
* Linux
* .NET

## Stap 2 - Configureren Azure SQL database
Om jouw Azure database geschikt te maken voor migraties vanuit GitHub Actions maken we een SQL user aan die beschikt over `db_owner` rechten. 

1. Open SQL Management Studio
2. Verbind met jouw database server ( jouw_gekozen_naam .database.windows.net) en maak gebruik van Azure - Universal with MFA. Zodat je met je Azure account deze database kunt beheren.


We gaan 2 logins aanmaken:
* migrator: verantwoordelijk voor het beheren van de SQL database schema
* app_user: de login die de applicatie kan gebruiken om gegevens op te halen/weg te schrijven

Vervang de wachtwoorden met door jouw verzonnen wachtwoorden en voer onderstaand SQL script uit op de `master`-database (zie dropdown boven in SQL Server Management Studio)

```
CREATE LOGIN [migrator]
WITH PASSWORD = '<your_sql_migrator_password>';  

CREATE LOGIN [app_user]
WITH PASSWORD = '<your_sql_app_user_password>'; 
```

Wijzig de verbinding naar de door jouw gekozen database naam (i.p.v. `master`) en koppel de zojuist aangemaakt logins aan users voor jouw database door onderstaand script uit te voeren.

```
/* Assign migrator as db_owner */
CREATE USER [migrator] FOR LOGIN [migrator];
ALTER ROLE [db_owner] ADD MEMBER [migrator];

/* Assign app_user as db_datawriter/db_datareader */
CREATE USER [app_user] FOR LOGIN [app_user];
ALTER ROLE [db_datawriter] ADD MEMBER [app_user];
ALTER ROLE [db_datareader] ADD MEMBER [app_user];
```

Indien je wilt controleren of je bovenstaande stappen goed hebt uitgevoerd maak je een nieuwe verbinding met je database server, maar nu met SQL username / password (i.p.v. Azure Universal). Als dit lukt heb je de gebruikersnamen goed geconfigureerd.

## Stap 3 - Maak credentials voor Azure
Ga naar https://portal.azure.com en open een Cloud Shell (menubalk boven)

Vervang `{subscription-id}`, `{resource-group}` en `{app-name}` met de gegevens van jouw azure app service en voor onderstaande code uit in de shell
```
az ad sp create-for-rbac --name {app-name} --role contributor --scopes /subscriptions/{subscription-id}/resourceGroups/{resource-group} --sdk-auth
```

Indien goed gedaan komt er een JSON terug. Bewaar deze JSON.

## Stap 4 - Configureer GitHub Repository Secrets
GitHub wordt verantwoordelijk voor het migreren van de database schema en het configureren van de verbinding van de App Service met de database. Daarom maken we een aantal secrets aan. Wanneer je met meerdere omgevingen werkt kun je ook gebruik maken van 'Environment Secrets'. Je dient dan wel nog de worflow zelf ook aan te passen. Om het simpel te houden gebruiken we nu alleen Repository Secrets en ondersteunen we dus maar 1 omgeving.

Via *Settings -> Secrets & Variables -> Actions* kun je nu de volgende secrets configureren:

| Name      | Value   |
|-----------|--------------|
| AZURE_APP_SERVICE_NAME  | de naam van jouw Azure App Service (zonder .azurewebsites.net)    |
| AZURE_CREDENTIALS | De JSON die je hebt ontvangen in Stap 3.     |
| AZURE_SQL_APP_USER_PASSWORD  | Het wachtwoord dat je hebt ingesteld i.p.v. `<your_sql_app_user_password>`     |
| AZURE_SQL_DATABASE_NAME  | de naam van de door jouw aangemaakte database     |
| AZURE_SQL_MIGRATOR_PASSWORD  | Het wachtwoord dat je hebt ingesteld i.p.v. `<your_sql_migrator_password>`     |
| AZURE_SQL_SERVER_NAME  | De naam van jouw Azure SQL server (zonder .database.windows.net)         |
