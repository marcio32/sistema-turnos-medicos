# Clase 2 — Challenge Práctico: CI/CD y Code Review

## Objetivo

Configurar herramientas de calidad de código (ESLint, Prettier, Conventional Commits) en el repositorio del Sistema de Turnos, crear un pipeline de CI con GitHub Actions y practicar code review cruzado con checklist.

## Duración: 40 minutos

## Equipos: Los mismos de Clase 1

---

## Paso a paso

### Parte 1: Configurar ESLint y Prettier (10 min)

1. Desde `main`, crear branch `feature/linting-setup`:
   ```bash
   git checkout main && git pull
   git checkout -b feature/linting-setup
   ```
2. Instalar dependencias:
   ```bash
   npm init -y
   npm install -D eslint prettier eslint-config-prettier @commitlint/cli @commitlint/config-conventional husky
   ```
3. Crear `.eslintrc.json` (extends recommended + prettier)
4. Crear `.prettierrc` (semi, singleQuote, tabWidth 2)
5. Crear `.commitlintrc.json` (extends conventional)

### Parte 2: Crear CI workflow (15 min)

1. Crear `.github/workflows/ci.yml`:
   ```yaml
   name: CI
   on:
     push:
       branches: [main]
     pull_request:
       branches: [main]
   jobs:
     quality:
       runs-on: ubuntu-latest
       steps:
         - uses: actions/checkout@v4
         - uses: actions/setup-node@v4
           with:
             node-version: '20'
             cache: 'npm'
         - run: npm ci
         - run: npx eslint . --ext .js,.ts,.tsx
         - run: npx prettier --check "**/*.{js,ts,tsx,json,md}"
   ```
2. Agregar script en `package.json`:
   ```json
   "scripts": {
     "lint": "eslint . --ext .js,.ts,.tsx",
     "format:check": "prettier --check \"**/*.{js,ts,tsx,json,md}\""
   }
   ```
3. Commitear y pushear:
   ```bash
   git add . && git commit -m "ci: add eslint, prettier and github actions workflow"
   git push -u origin feature/linting-setup
   ```

### Parte 3: Code Review cruzado (15 min)

1. Crear el PR en GitHub
2. El reviewer usa este checklist para evaluar:
   - [ ] El commit message sigue Conventional Commits
   - [ ] No hay errores de linting
   - [ ] Los archivos de configuración son consistentes entre sí
   - [ ] El workflow tiene los triggers correctos
   - [ ] No se suben archivos innecesarios (node_modules, etc.)
3. Dejar al menos un comentario constructivo con sugerencia
4. El autor responde, ajusta si es necesario, y mergea

---

## Entregable

- ESLint + Prettier configurados y funcionando localmente
- Conventional Commits activo con commitlint
- GitHub Actions workflow que corre lint + format check en cada PR
- Un code review completado con al menos un comentario

## Conexión con la Clase 3

Con el repositorio y CI funcionando, la próxima clase nos enfocaremos en organizar el backlog del proyecto usando GitHub Projects, escribiendo las historias de usuario que guiarán el desarrollo.
