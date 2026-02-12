# Test technique WeChooz 

Bienvenue dans le projet de test technique WeChooz !

## Instructions générales

Ce repo contient un test technique pour évaluer vos compétences en développement C#/.NET et React/TypeScript.
Vous trouverez ci-dessous les instructions pour réaliser ce test.

Ce test est à réaliser en autonomie. Vous devez appliquer des pratiques aussi proches que possible de celles que vous utiliseriez dans un projet professionnel (qualité de code, sécurité, performance notamment).

Pour commencer, forkez ce repo et cloner le projet sur votre machine locale.
Une fois terminé, vous pouvez soumettre votre travail en créant une pull request sur le repo d'origine.

## Objectif de l'application

WeChooz propose une activité de formation dans le domaine du dialogue social.
Nous proposons des formations en ligne et en présentiel, à destination des élus du CSE mais aussi des présidents de CSE.

Afin de fiabiliser la réservation de ces formations, nous souhaitons mettre en place un système de gestion des réservations.
Dans la 1ère version que vous allez développer, nous souhaitons permettre aux personnes souhaitant acheter une formation de visualiser les prohaines sessions disponibles.
Pour se faire, l'application dispose de deux parties :

1. Une interface d'administration pour gérer les sessions de formation, accessible via l'URL `/admin`,
2. une interface utilisateur pour visualiser les sessions de formation disponibles, accessible via l'URL `/`.

### Interface d'administration
L'interface d'administration require une authentification préalable. Dans le cadre de ce test, deux identifiants sont disponibles (sans mot de passe): `formation` et `sales`.
Ils permettent de se connecter avec les rôles du même nom.

Le rôle `formation` permet de gérer les cours et les sessions de formation et de gérer les participants à une session, tandis que le rôle `sales` permet uniquement de gérer les participants à une session de formation.

Un cours est défini par les informations suivantes :
- un nom,
- une description courte (aussi appelé chapo),
- une description longue textuelle en markdown,
- une durée (en jours),
- une population cible (élu ou président de CSE),
- une capacité maximale,
- un formateur (nom et prénom).

Une session est un cours donné à partir d'une date et avec un mode de délivrance (présentiel ou à distance), avec une liste de participants.

Un parcipant est défini par les informations suivantes :
- un nom,
- un prénom,
- une adresse e-mail,
- un nom d'entreprise.

### Interface publique
La partie publique de l'application permet de visualiser les sessions de formation disponibles, ainsi que les filtrer selon les critères suivants :
- la population cible (élu ou président de CSE),
- le mode de délivrance (présentiel ou à distance),
- la date de début de la session (avant, après, entre deux dates).

La visualisation des sessions de formation se fait sous forme de liste, avec les informations suivantes :
- le nom du cours,
- la description courte,
- la population cible,
- la date de début de la session,
- la durée de la session,
- le mode de délivrance (présentiel ou à distance),
- le nombre de places restantes,
- le formateur.

Il doit également être possible de visualiser les détails d'une session de formation, permettant de visualiser également la description longue du cours, formaté en HTML.

## Comment démarrer le projet

### Informations générales
Le projet a été créé en partant d'une structure de base via [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview) et contenant les projects suivants :
- `WeChooz.TechAssessment.AppHost` : le projet hôte [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview).
Ce projet doit être défini comme projet de démarrage,
- `WeChooz.TechAssessment.Database.SqlServer` : le [projet base de donnée](https://learn.microsoft.com/en-us/sql/ssdt/sql-server-data-tools?view=sql-server-ver17) contenant la définition de la base de donnée associée au projet. Il faut manuellement publier la base de données après chaque changement de celui-ci,
- `WeChooz.TechAssessment.Web` : le projet principal de l'application, contenant les API ainsi que le front-end, suivant le pattern [backend for frontend](https://learn.microsoft.com/en-us/azure/architecture/patterns/backends-for-frontends).
Ce projet contient donc le code React, bundlé avec Vite. Il existe deux points d'entrée : `ClientApp/src/index.tsx` pour l'interface publique et `ClientApp/src/admin/index.tsx` pour l'interface d'administration.

Vous pouvez créer d'autres projets si vous le souhaitez, mais il est demandé de respecter la structure de base du projet.

### Prérequis

Afin de fonctionner, le projet nécessite les prérequis suivants :
- .NET 10.0,
- Visual Studio 2026 Community ou VS Code,
- un runtime de container (Docker ou podman),
- [VoltaJS](https://docs.volta.sh/guide/getting-started) pour la gestion des versions de Node.js (optionnel, mais recommandé),

Le frontend utilise React 19.1 avec TypeScript 5.9 et il est demandé d'utiliser la librairie [Mantine](https://mantine.dev/) pour la partie UI.
D'autres librairies (dans le backend ou le frontend) ont été installées, n'hésitez pas à les explorer.

### Aspire
Le projet Aspire définit les ressources suivantes :
- un serveur SQL Server (lancé via un container écoutant sur le port 5678 avec les identifiants `sa`/`yourStrong(!)Password)`), ainsi qu'une base de donnée `formation`
- un serveur Redis (lancé via un container), ainsi qu'un side container exécutant Redis Insight,

Le lancement du projet va donc créer les ressources nécessaires à l'exception de la mise à jour du schéma de base de données qui doit être fait manuellement en publiant le projet `WeChooz.TechAssessment.Database.SqlServer`. Le 1er lancement peut prendre un peu de temps, le temps que les containers soient créés et démarrés.