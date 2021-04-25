## Builds Status

Client Léger (Android)
[![Build Status](https://dev.azure.com/loicbellemare-alford/Kotlin%20TS/_apis/build/status/Build%20%26%20validate%20(Android)?branchName=dev)](https://dev.azure.com/loicbellemare-alford/Kotlin%20TS/_build/latest?definitionId=1&branchName=dev)
[![Build status](https://build.appcenter.ms/v0.1/apps/c32573f3-b774-4fe6-bb41-3d5a79fbada8/branches/dev/badge)](https://appcenter.ms)


Client Lourd (WPF)
[![Build Status](https://dev.azure.com/loicbellemare-alford/Kotlin%20TS/_apis/build/status/loic294.log3900?branchName=dev)](https://dev.azure.com/loicbellemare-alford/Kotlin%20TS/_build/latest?definitionId=1&branchName=dev)
[![Release Status](https://vsrm.dev.azure.com/loicbellemare-alford/_apis/public/Release/badge/42951ab9-bc28-48d5-b7ff-d0a52e40647d/1/1)](https://dev.azure.com/loicbellemare-alford/Kotlin%20TS/_release?_a=releases&view=mine&definitionId=1)


Repo Mirror
[![Release Status](https://vsrm.dev.azure.com/loicbellemare-alford/_apis/public/Release/badge/42951ab9-bc28-48d5-b7ff-d0a52e40647d/2/2)](https://dev.azure.com/loicbellemare-alford/Kotlin%20TS/_release?view=all&_a=releases&definitionId=2)

## Convention des noms des branches

`action/alias/client/nom-de-la-tache`

Exemple: `feature/lobel/wpf/add-firebase-to-project`

Action peut-être:
- feature
- hotfix
- bug

L'alias est celui de Poly (exemple `lobel` pour Loïc)

Le client peut-être:
- android
- wpf
- server
- ci
- all

## Convention des noms de commits (même que les noms de tâches sur Redmind)

### Nom du commit

`[TACHE] (client) Description de la tâche`

Exemple: `[DEV] (Android) Added models for PolyChat`

Tâche peut-être:
- DEV
- FIX
- REF

Le client peut-être:
- Android
- WPF
- Server
- CI
- All

### Description du commit

- id de Redmine si elle existe
- description détaillée au besoin

Le nom est une description de la tâche et contient l'ID de la tâche Redmind si elle existe.

## Quel quelques liens utiles

- [Liens vers Teams](https://teams.microsoft.com/l/team/19%3a3b2a7ab1f1a1487b85212e5c21f54b99%40thread.skype/conversations?groupId=0c75f7f6-917e-4315-be69-e05051dc67dd&tenantId=d2ee49eb-e94b-49d7-a34a-2fb97a949bfc)
- [Liens vers le Google Drive d'équipe](https://drive.google.com/drive/folders/16F9vjkL6jg5niXBUqSAfvW2dpr_pqPbx?usp=sharing)
- [Redmine](https://redmine.gi.polymtl.ca/)
- [Build pipeline d'Android](https://dev.azure.com/loicbellemare-alford/Kotlin%20TS/_build?definitionId=1&_a=summary)
- [Build pipeline de WPF](https://dev.azure.com/loicbellemare-alford/Kotlin%20TS/_build?definitionId=2&_a=summary)
- [Deployment d'Android](https://appcenter.ms/users/loic.bellemare.alford/apps/Kotlin-TS)
- [Deployment de WPF](https://appcenter.ms/users/loic.bellemare.alford/apps/Kotlin-TS-WPF)
