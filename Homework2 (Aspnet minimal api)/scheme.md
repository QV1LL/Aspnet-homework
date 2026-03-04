entity Post:
- id (Guid)
- title (string)
- Content (string
- imageUrl (string)
- slug (string[])
- categories (Category[])

entity Category:
- id (Guid)
- name (string)
- posts (Post[])
