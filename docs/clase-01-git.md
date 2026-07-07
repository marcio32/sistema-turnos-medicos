# Clase 1 — Lab Práctico: Git Colaborativo

## Objetivo

Crear el repositorio del proyecto Sistema de Turnos Médicos en GitHub, configurar branch protection y practicar flujo colaborativo con feature branches, incluyendo resolución de conflictos intencionados.

## Duración: 40 minutos

## Equipos: 2-3 personas

---

## Paso a paso

### Parte 1: Crear el repositorio (10 min)

1. Un integrante crea el repo `sistema-turnos-medicos` en GitHub (público, con README)
2. Agregar a los compañeros como colaboradores (Settings → Collaborators)
3. Todos clonan el repo localmente:
   ```bash
   git clone https://github.com/<org>/sistema-turnos-medicos.git
   cd sistema-turnos-medicos
   ```
4. Crear la estructura inicial en `main`:
   ```bash
   mkdir -p frontend backend docs
   echo "# Sistema de Turnos Médicos" > README.md
   git add . && git commit -m "chore: initial project structure"
   git push origin main
   ```

### Parte 2: Configurar branch protection (5 min)

1. Ir a Settings → Branches → Add rule
2. Branch name pattern: `main`
3. Activar:
   - ✅ Require a pull request before merging
   - ✅ Require approvals (1)
   - ✅ Do not allow bypassing the above settings
4. Guardar la regla

### Parte 3: Feature branches y PRs (15 min)

Cada integrante trabaja en una feature branch distinta:

- **Persona A**: `feature/add-gitignore`
  ```bash
  git checkout -b feature/add-gitignore
  # Crear .gitignore con node_modules/, dist/, .env
  git add . && git commit -m "chore: add gitignore"
  git push -u origin feature/add-gitignore
  ```
- **Persona B**: `feature/add-editorconfig`
  ```bash
  git checkout -b feature/add-editorconfig
  # Crear .editorconfig con indent_style = space, indent_size = 2
  git add . && git commit -m "chore: add editorconfig"
  git push -u origin feature/add-editorconfig
  ```
- Crear Pull Requests desde GitHub y pedir review al compañero
- Aprobar y mergear la primera PR

### Parte 4: Resolver conflictos (10 min)

1. **Ambas personas** editan el mismo archivo `README.md` en sus branches (agregar secciones distintas en la misma línea)
2. La segunda persona al intentar mergear verá un conflicto
3. Resolver el conflicto localmente:
   ```bash
   git checkout feature/mi-branch
   git pull origin main
   # Resolver conflictos en el editor (aceptar ambos cambios)
   git add README.md
   git commit -m "fix: resolve merge conflict in README"
   git push
   ```
4. Completar el merge del PR

---

## Entregable

- Repositorio en GitHub con branch protection activa
- Al menos 2 PRs mergeadas (una por integrante)
- Un conflicto resuelto documentado en el historial de commits

## Conexión con la Clase 2

En la próxima clase configuraremos ESLint, Prettier y un pipeline de CI sobre este mismo repositorio, automatizando la calidad del código que pusheamos.
