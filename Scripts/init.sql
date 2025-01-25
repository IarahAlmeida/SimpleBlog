\set db `echo "$POSTGRES_DB"`
\set user `echo "$POSTGRES_USER"`
\set password `echo "$POSTGRES_PASSWORD"`
\set schema `echo "$POSTGRES_SCHEMA"`

\c :db

CREATE SCHEMA IF NOT EXISTS :schema;
ALTER SCHEMA :schema OWNER TO :user;