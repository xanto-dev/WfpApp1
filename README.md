# Lancer l'application WpfApp1

## Prérequis

- Windows
- Visual Studio 2022 (avec la charge de travail de développement de bureau .NET)
- .NET 8.0 SDK installé

Vérifier l'installation :

```bash
dotnet --version
```

## Lancement rapide (CLI)

Depuis la racine du projet (le dossier contenant `WpfApp1.sln`) :

```bash
dotnet restore
dotnet run --project WpfApp1/WpfApp1.csproj
```

## Lancement avec Visual Studio

1.  Ouvrir `WpfApp1.sln` avec Visual Studio.
2.  Choisir la configuration **Debug**.
3.  Définir `WpfApp1` comme projet de démarrage (clic-droit sur le projet > Définir comme projet de démarrage).
4.  Lancer avec **F5** ou le bouton de démarrage.

## Problèmes fréquents

-   **`dotnet` non reconnu :** Assurez-vous que le SDK .NET 8 est bien installé et que son chemin est ajouté à la variable d'environnement PATH de votre système. Redémarrez votre terminal après l'installation.
-   **Erreur de restauration des paquets (NuGet) :** Exécutez `dotnet restore` à la racine du projet pour télécharger les dépendances.
-   **Mauvaise version du SDK :** Si vous avez plusieurs versions du SDK .NET, assurez-vous que la version 8.0 est utilisée. Vous pouvez lister les SDK installés avec `dotnet --list-sdks`.
