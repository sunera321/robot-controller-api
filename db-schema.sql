-- Table for user accounts.
CREATE TABLE users (
    -- Unique user id.
    id SERIAL PRIMARY KEY,
    -- User email address.
    email VARCHAR(255) NOT NULL,
    -- User first name.
    firstname VARCHAR(100) NOT NULL,
    -- User last name.
    lastname VARCHAR(100) NOT NULL,
    -- Hashed password value.
    passwordhash TEXT NOT NULL,
    -- Optional user notes.
    description TEXT,
    -- User role for access control.
    role VARCHAR(50),
    -- When the user was created.
    createddate TIMESTAMP NOT NULL,
    -- When the user was last changed.
    modifieddate TIMESTAMP NOT NULL
);

-- Table for robot commands.
CREATE TABLE robot_commands (
    -- Unique command id.
    id SERIAL PRIMARY KEY,
    -- Command name.
    name VARCHAR(100) NOT NULL,
    -- Optional command notes.
    description TEXT,
    -- True when this command moves the robot.
    ismovecommand BOOLEAN NOT NULL,
    -- When the command was created.
    createddate TIMESTAMP NOT NULL,
    -- When the command was last changed.
    modifieddate TIMESTAMP NOT NULL
);

-- Table for maps.
CREATE TABLE maps (
    -- Unique map id.
    id SERIAL PRIMARY KEY,
    -- Map name.
    name VARCHAR(100) NOT NULL,
    -- Optional map notes.
    description TEXT,
    -- Number of rows in the map grid.
    rows INTEGER NOT NULL,
    -- Number of columns in the map grid.
    columns INTEGER NOT NULL,
    -- When the map was created.
    createddate TIMESTAMP NOT NULL,
    -- When the map was last changed.
    modifieddate TIMESTAMP NOT NULL
);