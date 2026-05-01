# Frontend (Angular)

Basic Angular frontend for Product CRUD.

## Run

1. Install dependencies:
   ```bash
   yarn install
   ```
2. Ensure backend API is running on `http://localhost:5199`.
3. Start the dev server:
   ```bash
   yarn start
   ```

`yarn start` runs Angular with `proxy.conf.json`, so browser requests to `/api/*` stay same-origin (`http://localhost:4200`) and are proxied to the backend API.

The page supports:
- List products
- Create a product
- Edit a product
- Delete a product
