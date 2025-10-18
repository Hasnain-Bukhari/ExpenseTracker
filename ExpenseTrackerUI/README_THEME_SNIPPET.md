Theme system
============

To add a new theme variant:

1. Edit `src/styles/themes.ts` and add a new key with `light` and `dark` ThemeDefinition objects. Use existing tokens: `primary`, `secondary`, `surface`, `background`, `text`, `cardBg`, `cardGlass`, `muted`, `accent`, `chart1`, `chart2`, `shadow1`, `radiusLg`, `transitionFast`.
2. Optionally add a label in `themeLabels` for display name and icon.
3. No other code changes required — `ThemeSelector` and `themeStore` read from the registry dynamically.

Persistence: user selection is stored in `localStorage` under `theme:selected` and `theme:isDark`.

Behavior: The dark/light toggle flips the brightness of the selected theme variant (each theme has light & dark definitions).
