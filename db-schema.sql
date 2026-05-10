CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    email VARCHAR(255) NOT NULL,
    firstname VARCHAR(100) NOT NULL,
    lastname VARCHAR(100) NOT NULL,
    passwordhash TEXT NOT NULL,
    description TEXT,
    role VARCHAR(50),
    createddate TIMESTAMP NOT NULL,
    modifieddate TIMESTAMP NOT NULL
);

CREATE TABLE robot_commands (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    ismovecommand BOOLEAN NOT NULL,
    createddate TIMESTAMP NOT NULL,
    modifieddate TIMESTAMP NOT NULL
);

CREATE TABLE maps (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    rows INTEGER NOT NULL,
    columns INTEGER NOT NULL,
    createddate TIMESTAMP NOT NULL,
    modifieddate TIMESTAMP NOT NULL
);