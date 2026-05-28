# Page d'édition d'événement (EditEventPage)

## Vue d'ensemble
La page EditEventPage permet de modifier les détails d'un événement GEIPAN existant. Elle est accessible depuis AdminPage via le bouton "Éditer" pour chaque événement.

## Architecture

### EditEventViewModel
- **Fichier** : `PAN/ViewModels/EditEventViewModel.cs`
- **Responsabilités** :
  - Charger l'événement existant via son ID
  - Charger les listes de lookup (Classements, Types, Phénomènes)
  - Gérer la sauvegarde des modifications en base de données
  - Gérer la navigation (retour arrière, annulation)

### EditEventPage (View + CodeBehind)
- **Fichier XAML** : `PAN/Views/EditEventPage.xaml`
- **CodeBehind** : `PAN/Views/EditEventPage.xaml.cs`
- **Responsabilités** :
  - Afficher le formulaire d'édition
  - Extraire l'ID de l'événement depuis les query parameters
  - Déclencher le chargement des données

## Flux de navigation

1. **AdminPage** → Clic sur bouton "Éditer"
   ```csharp
   Command="{Binding Source={x:Reference AdminPageView}, Path=BindingContext.EditCommand}"
   CommandParameter="{Binding IdEvenement}"
   ```

2. **AdminViewModel.EditAsync()** → Navigation
   ```csharp
   await NavigationService.GoToAsync($"evenementdetail?id={idEvenement}");
   ```

3. **Shell** → Route vers EditEventPage
   ```xaml
   <ShellContent
	   Title="Modifier un événement"
	   Route="evenementdetail"
	   ContentTemplate="{DataTemplate views:EditEventPage}" />
   ```

4. **EditEventPage.OnNavigatedTo()** → Extraction de l'ID et chargement
   - Extrait `?id={idEvenement}` de l'URL
   - Appelle `LoadEventCommand.ExecuteAsync(id)`

5. **EditEventViewModel.LoadEvent()** → Chargement des données
   - Récupère l'événement avec ses relations
   - Charge les listes de lookup

## Champs éditables

| Champ | Type | Description |
|-------|------|-------------|
| Descriptif | Editor | Description courte de l'événement (max 500 caractères) |
| Date/Heure | DatePicker + TimePicker | Date et heure d'observation |
| Classification | Picker | Classification GEIPAN (Classements) |
| Type | Picker | Type d'événement |
| Phénomène | Picker | Phénomène observé |
| Compte rendu | Editor | Compte rendu détaillé (max 2000 caractères) |
| Latitude | Entry | Coordonnée latitude |
| Longitude | Entry | Coordonnée longitude |
| Objet mouvant | CheckBox | Indique si l'objet était mouvant |

## Enregistrement et validation

### Sauvegarde
1. Utilisateur clique "Enregistrer"
2. `SaveCommand` est exécuté
3. L'événement est mis à jour en base de données via EntityFrameworkCore
4. Message de succès affiché
5. Navigation retour vers AdminPage

### Annulation
- Clic "Annuler" → Retour sans sauvegarde

## Gestion des erreurs

- **Événement non trouvé** : Alerte affichée + retour à AdminPage
- **Erreur de chargement** : Alerte avec message d'erreur
- **Erreur de sauvegarde** : Alerte avec message d'erreur détaillé

## Dépendances d'injection

```csharp
builder.Services.AddTransient<EditEventViewModel>();
builder.Services.AddTransient<EditEventPage>();
```

## Points d'amélioration possibles

1. **Validation des données** : Ajouter une validation côté client avant sauvegarde
2. **Confirmation avant annulation** : Demander confirmation si des modifications non sauvegardées
3. **Mode édition contorlé** : Désactiver les champs en cas d'erreur de chargement
4. **Historique** : Ajouter un système de versioning des modifications
5. **Optimisation des performances** : Utiliser AsNoTracking pour les lookups (données en lecture seule)

## Teste manuel

1. Accéder à la page Administration
2. Cliquer sur "Éditer" pour un événement
3. Modifier les champs
4. Cliquer "Enregistrer"
5. Vérifier que les données sont sauvegardées en base
6. Retourner à l'administration et vérifier que les modifications sont visibles
