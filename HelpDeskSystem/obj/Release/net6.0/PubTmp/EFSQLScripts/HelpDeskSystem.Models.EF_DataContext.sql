CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420154256_Migration1') THEN
    CREATE TABLE "Accounts" (
        id serial NOT NULL,
        fullname text NOT NULL,
        company text NULL,
        workemail text NULL,
        password text NULL,
        CONSTRAINT "PK_Accounts" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420154256_Migration1') THEN
    CREATE TABLE "Contacts" (
        id serial NOT NULL,
        fullname text NOT NULL,
        email text NULL,
        bio text NULL,
        "phoneNumber" text NULL,
        company text NULL,
        country integer NULL,
        city text NULL,
        facebook text NULL,
        twitter text NULL,
        linkedin text NULL,
        github text NULL,
        CONSTRAINT "PK_Contacts" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420154256_Migration1') THEN
    CREATE TABLE "Countrys" (
        id serial NOT NULL,
        name text NOT NULL,
        CONSTRAINT "PK_Countrys" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230420154256_Migration1') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230420154256_Migration1', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421041125_Migration2') THEN
    CREATE TABLE "ConfigMails" (
        id serial NOT NULL,
        "yourName" text NOT NULL,
        email text NULL,
        password text NULL,
        incoming text NULL,
        "incomingPort" integer NULL,
        outgoing text NULL,
        "outgoingPort" integer NULL,
        CONSTRAINT "PK_ConfigMails" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421041125_Migration2') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230421041125_Migration2', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421072258_Migration3') THEN
    CREATE TABLE "EmailInfos" (
        id serial NOT NULL,
        "from" text NOT NULL,
        "to" text NULL,
        cc text NULL,
        bcc text NULL,
        subject text NULL,
        "textBody" text NULL,
        CONSTRAINT "PK_EmailInfos" PRIMARY KEY (id)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421072258_Migration3') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230421072258_Migration3', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421075134_Migration4') THEN
    ALTER TABLE "EmailInfos" ALTER COLUMN "from" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421075134_Migration4') THEN
    ALTER TABLE "EmailInfos" ADD date timestamp with time zone NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421075134_Migration4') THEN
    ALTER TABLE "EmailInfos" ADD "messageId" text NOT NULL DEFAULT '';
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421075134_Migration4') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230421075134_Migration4', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421083308_Migration5') THEN
    ALTER TABLE "EmailInfos" ALTER COLUMN "messageId" DROP NOT NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421083308_Migration5') THEN
    ALTER TABLE "EmailInfos" ADD "idConfigEmail" integer NOT NULL DEFAULT 0;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421083308_Migration5') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230421083308_Migration5', '7.0.5');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421100251_Migration6') THEN
    ALTER TABLE "EmailInfos" ADD "fromName" text NULL;
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20230421100251_Migration6') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20230421100251_Migration6', '7.0.5');
    END IF;
END $EF$;
COMMIT;

