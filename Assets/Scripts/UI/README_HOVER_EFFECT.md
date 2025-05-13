# Menu Item Hover Effect

Ce script permet d'ajouter un effet de survol sur vos éléments de menu. Quand le joueur passe la souris sur un élément, le texte change progressivement vers la couleur rouge (ou une autre couleur configurable).

## Installation

1. Le script `MenuItemHoverEffect.cs` a été ajouté au projet
2. `MainMenuManager.cs` a été mis à jour pour appliquer cet effet automatiquement aux boutons du menu principal

## Comment utiliser

### Application automatique à tous les boutons du menu principal

Le `MainMenuManager` applique automatiquement l'effet de survol à tous les boutons configurés (newgame, continue, settings, quit). Vous n'avez rien à faire de plus si vous utilisez cette approche.

### Application manuelle à d'autres éléments

Pour ajouter cet effet à d'autres éléments de l'interface:

1. Sélectionnez le GameObject contenant le texte dans l'inspecteur
2. Cliquez sur "Add Component"
3. Recherchez et ajoutez "Menu Item Hover Effect"
4. Configurez les propriétés selon vos préférences:
   - **Hover Color**: La couleur vers laquelle le texte va changer lors du survol (rouge par défaut)
   - **Transition Duration**: La durée de la transition en secondes (0.2s par défaut)
   - **Transition Curve**: La courbe d'animation pour contrôler la progression de la transition

## Compatibilité

Ce script fonctionne avec:
- TextMeshPro (recommandé)
- Legacy Text UI

Le script détecte automatiquement le type de composant texte utilisé.

## Script d'immersion environnementale

Un script `EnvironmentImmersion.cs` a également été ajouté pour créer une atmosphère plus immersive dans votre jeu. Ce script:

1. Ajoute des sons ambiants aléatoires (craquements, etc.)
2. Fait scintiller les lumières de manière aléatoire
3. Ajoute des effets de post-traitement dynamiques
4. Crée des effets "parasites" sur les écrans TV

### Nouveaux paramètres audio

Des paramètres audio ont été ajoutés au script d'immersion:

1. **Master Volume**: Volume général qui affecte tous les sons (0-1)
2. **Creaking Sound Volume**: Volume des sons de craquement (0-1)
3. **Ambient Sound Volume**: Volume des sons ambiants (0-1)
4. **Wind Sound Volume**: Volume du son de vent (0-1)
5. **Pitch Variation**: Variation de hauteur des sons (0-0.5)

Vous pouvez modifier ces paramètres:
- Directement dans l'inspecteur
- Via du code en appelant `UpdateAudioVolumes(master, creaking, ambient, wind)`
- Via l'interface utilisateur fournie dans `AudioSettingsUI.cs`

### Interface utilisateur pour les paramètres audio

Un script `AudioSettingsUI.cs` a été créé pour permettre aux joueurs d'ajuster les paramètres audio:

1. Ajoutez ce script à un GameObject contenant des sliders UI
2. Assignez les références aux sliders et aux labels correspondants
3. Connectez le script au composant `EnvironmentImmersion`

Les paramètres audio sont automatiquement sauvegardés dans PlayerPrefs.

### Comment utiliser le script d'immersion

1. Ajoutez le composant `EnvironmentImmersion` à un GameObject dans votre scène
2. Configurez les références aux lumières, effets sonores et volumes de post-traitement
3. Vous pouvez appeler `IncreaseTension(amount, duration)` depuis d'autres scripts pendant les moments importants du jeu

## Aide supplémentaire

Si vous avez besoin d'aide ou si vous souhaitez personnaliser davantage ces scripts, n'hésitez pas à me contacter. 