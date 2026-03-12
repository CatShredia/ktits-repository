## Second Practise

### Schema Objects

- **Film** (id, name, description, release_date, genre_id, image_url): CRUD, Read(id), Average_Rating;
- **Genre** (id, name, description): CRUD, Read(id);
- **Rating** (id, value, film_id): CRUD

#### Schema Roles

- **admin**: film CRUD, Read(id), Average_Rating; genre CRUD;rating CRUD;
- **client**: film Read, Read(id), Average_Rating; genre Read, Read(id);rating CRUD, Read(id);

### Client features

- forms and views for all routes;
- sort, filter for films;
- additional: creating public link for images
