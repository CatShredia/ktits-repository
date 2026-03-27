## Hi, this is public repository to KTITS works

​users table:
| column_name | data_type | is_nullable | column_default |
| --------------------------- | ------------------------ | ----------- | ----------------------- |
| instance_id | uuid | YES | null |
| created_at | timestamp with time zone | NO | now() |
| id | uuid | NO | null |
| aud | character varying | YES | null |
| full_name | text | YES | null |
| avatar | text | YES | null |
| role | character varying | YES | null |
| email | character varying | YES | null |
| email | text | NO | null |
| encrypted_password | character varying | YES | null |
| password | text | NO | null |
| id | uuid | NO | auth.uid() |
| email_confirmed_at | timestamp with time zone | YES | null |
| invited_at | timestamp with time zone | YES | null |
| confirmed | boolean | NO | false |
| confirmation_token | character varying | YES | null |
| confirmation_sent_at | timestamp with time zone | YES | null |
| recovery_token | character varying | YES | null |
| recovery_sent_at | timestamp with time zone | YES | null |
| email_change_token_new | character varying | YES | null |
| email_change | character varying | YES | null |
| email_change_sent_at | timestamp with time zone | YES | null |
| last_sign_in_at | timestamp with time zone | YES | null |
| raw_app_meta_data | jsonb | YES | null |
| raw_user_meta_data | jsonb | YES | null |
| is_super_admin | boolean | YES | null |
| created_at | timestamp with time zone | YES | null |
| updated_at | timestamp with time zone | YES | null |
| phone | text | YES | NULL::character varying |
| phone_confirmed_at | timestamp with time zone | YES | null |
| phone_change | text | YES | ''::character varying |
| phone_change_token | character varying | YES | ''::character varying |
| phone_change_sent_at | timestamp with time zone | YES | null |
| confirmed_at | timestamp with time zone | YES | null |
| email_change_token_current | character varying | YES | ''::character varying |
| email_change_confirm_status | smallint | YES | 0 |
| banned_until | timestamp with time zone | YES | null |
| reauthentication_token | character varying | YES | ''::character varying |
| reauthentication_sent_at | timestamp with time zone | YES | null |
| is_sso_user | boolean | NO | false |
| deleted_at | timestamp with time zone | YES | null |
| is_anonymous | boolean | NO | false |

products:
| column_name | data_type | is_nullable | column_default |
| ----------- | --------------------------- | ----------- | -------------- |
| id | bigint | NO | null |
| user_id | uuid | NO | auth.uid() |
| category_id | bigint | YES | null |
| name | text | NO | null |
| image | text | YES | null |
| description | text | YES | null |
| price_cents | bigint | YES | null |
| currency | text | YES | null |
| stock | bigint | YES | null |
| is_active | boolean | YES | null |
| created_at | timestamp without time zone | NO | null |

product_categories:
| column_name | data_type | is_nullable | column_default |
| ----------- | ------------------------ | ----------- | -------------- |
| id | bigint | NO | null |
| created_at | timestamp with time zone | NO | now() |
| name | character varying | NO | null |
| image | text | YES | null |

notifications:
| column_name | data_type | is_nullable | column_default |
| ----------- | ------------------------ | ----------- | ----------------- |
| id | bigint | NO | null |
| created_at | timestamp with time zone | NO | now() |
| user_id | uuid | NO | gen_random_uuid() |
| title | text | YES | null |
| body | text | YES | null |
| status | text | YES | null |
| read_at | boolean | YES | null |

Сделай для всех таблиц SQL запрос удаления всех данных и вставки вместо них тестовых (в каждую таблицу минимум - 5).
