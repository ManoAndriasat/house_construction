
DROP VIEW IF EXISTS vue_payement_mensuel CASCADE;
DROP VIEW IF EXISTS vue_payement_annuel CASCADE;
DROP VIEW IF EXISTS vue_payement_final CASCADE;
DROP VIEW IF EXISTS vue_payement_details CASCADE;
DROP VIEW IF EXISTS vue_total_devise CASCADE;
DROP VIEW IF EXISTS vue_payement_devise CASCADE;
DROP VIEW IF EXISTS vue_type_maison CASCADE;
DROP VIEW IF EXISTS vue_devis CASCADE;

DROP TABLE IF EXISTS payement CASCADE;
DROP TABLE IF EXISTS details_devise_client CASCADE;
DROP TABLE IF EXISTS finition_devise_client CASCADE;
DROP TABLE IF EXISTS devise_client CASCADE;
DROP TABLE IF EXISTS travaux CASCADE;
DROP TABLE IF EXISTS categorie_travaux CASCADE;
DROP TABLE IF EXISTS admin CASCADE;
DROP TABLE IF EXISTS finition CASCADE;
DROP TABLE IF EXISTS type_maison CASCADE;
DROP TABLE IF EXISTS users CASCADE;
DROP TABLE IF EXISTS devis_type_maison CASCADE;

DROP SEQUENCE IF EXISTS payement_id CASCADE;
DROP SEQUENCE IF EXISTS devise_client_id CASCADE;
DROP SEQUENCE IF EXISTS finition_id CASCADE;
DROP SEQUENCE IF EXISTS type_maison_id CASCADE;


create table users (
    numero varchar(20) primary key not null
);
insert into users values('0340590098');

create table admin(
    id varchar(20) primary key not null,
    nom varchar(20) not null,
    mdp varchar not null
);
insert into admin values('ADM1','mano','mano');

create table finition (
    id varchar(20) primary key not null,
    nom varchar(20) not null,
    taux decimal not null
);
create sequence finition_id;


create table type_maison (
    id varchar(20) primary key not null,
    nom varchar(20) not null,
    details varchar(400) not null,
    duree_construction decimal not null,
    surface decimal not null
);
create sequence type_maison_id;



CREATE TABLE categorie_travaux (
    id_categorie VARCHAR(50) PRIMARY KEY,
    designation VARCHAR(255) NOT NULL
);

CREATE TABLE travaux (
    id_travaux VARCHAR(50) PRIMARY KEY,
    id_categorie VARCHAR(50) REFERENCES categorie_travaux(id_categorie),
    designation VARCHAR(255) NOT NULL,
    unite VARCHAR(50),
    pu DECIMAL(10, 2),
    FOREIGN KEY (id_categorie) REFERENCES categorie_travaux(id_categorie)
);

CREATE TABLE devis_type_maison (
    id_type_maison VARCHAR(50) ,
    id_travaux VARCHAR(50) REFERENCES travaux(id_travaux),
    quantite DECIMAL,
    FOREIGN KEY (id_travaux) REFERENCES travaux(id_travaux)
);



CREATE VIEW vue_devis AS
SELECT
    ct.designation AS categorie_travaux,
    t.designation AS designation_travaux,
    t.unite AS unite_travaux,
    t.pu AS pu_travaux,
    dtm.quantite AS quantite_devis,
    dtm.id_type_maison AS type_maison,
    (t.pu * dtm.quantite) AS total_devis
FROM
    categorie_travaux ct
JOIN travaux t ON ct.id_categorie = t.id_categorie
JOIN devis_type_maison dtm ON t.id_travaux = dtm.id_travaux;



create sequence devise_client_id;
CREATE TABLE devise_client (
    id VARCHAR(20) PRIMARY KEY,
    users VARCHAR(20),
    type_maison VARCHAR(255),
    finition VARCHAR(255),
    date DATE,
    designation VARCHAR(255),
    lieu VARCHAR(255),
    FOREIGN KEY (users) REFERENCES users(numero),
    FOREIGN KEY (type_maison) REFERENCES type_maison(id),
    FOREIGN KEY (finition) REFERENCES finition(id)
);

create sequence payement_id;
create table Payement (
    id_payement VARCHAR(50),
    id_devise_client VARCHAR(50),
    date DATE,
    montant DECIMAL,
    reference VARCHAR(50),
    FOREIGN KEY (id_devise_client) REFERENCES devise_client(id)
);

create table finition_devise_client (
    id_devise_client VARCHAR(50),
    id varchar(20) not null,
    nom varchar(20) not null,
    taux decimal not null
);

CREATE TABLE details_devise_client (
    id_devise_client VARCHAR(50),
    categorie_travaux VARCHAR(255) NOT NULL,
    designation_travaux VARCHAR(255) NOT NULL,
    unite_travaux VARCHAR(50),
    pu_travaux DECIMAL,
    quantite_devis DECIMAL,
    type_maison VARCHAR(50),
    total DECIMAL,
    FOREIGN KEY (id_devise_client) REFERENCES devise_client(id)
);

CREATE or replace view vue_total_devise AS
SELECT
    id_devise_client,
    sum(total) as total
FROM details_devise_client
GROUP BY id_devise_client;

CREATE or replace view vue_payement_devise AS
SELECT
    id_devise_client,
    sum(montant) as total
FROM payement 
GROUP BY id_devise_client;


CREATE OR REPLACE VIEW vue_payement_details AS
SELECT
    d.id_devise_client,
    dc.users,
    d.total AS total,
    COALESCE(p.total, 0) AS payer,
    d.total - COALESCE(p.total, 0) AS reste
FROM
    vue_total_devise d
LEFT JOIN vue_payement_devise p ON p.id_devise_client = d.id_devise_client
JOIN devise_client dc ON d.id_devise_client = dc.id;

CREATE VIEW vue_type_maison AS
SELECT tm.id, tm.nom, tm.details, tm.duree_construction, tm.surface,
       COALESCE(SUM(dtm.quantite * t.pu), 0) AS total
FROM type_maison tm
LEFT JOIN devis_type_maison dtm ON tm.id = dtm.id_type_maison
LEFT JOIN travaux t ON dtm.id_travaux = t.id_travaux
GROUP BY tm.id, tm.nom, tm.details, tm.duree_construction, tm.surface;

CREATE OR REPLACE VIEW vue_payement_final AS
SELECT
    vpd.id_devise_client,
    vpd.users,
    (vpd.total + vpd.total * f.taux/100) AS total,
    ((vpd.total + vpd.total * f.taux/100) - vpd.payer) AS reste,
    vpd.payer,
    dc.date,
    dc.designation,
    dc.finition,
    dc.type_maison,
    f.taux AS taux_finition,
    tm.duree_construction
FROM
    vue_payement_details vpd
JOIN devise_client dc ON vpd.id_devise_client = dc.id
JOIN finition_devise_client f ON vpd.id_devise_client = f.id_devise_client
JOIN type_maison tm ON dc.type_maison = tm.id;



CREATE OR REPLACE VIEW vue_payement_mensuel AS
SELECT
    DATE_TRUNC('month', dc.date) AS mois,
    SUM(vpf.total) AS somme
FROM
    vue_payement_final vpf
JOIN devise_client dc ON vpf.id_devise_client = dc.id
GROUP BY
    DATE_TRUNC('month', dc.date);

CREATE OR REPLACE VIEW vue_payement_annuel AS
SELECT
    DATE_TRUNC('year', dc.date) AS annee,
    SUM(vpf.total) AS somme
FROM
    vue_payement_final vpf
JOIN devise_client dc ON vpf.id_devise_client = dc.id
GROUP BY
    DATE_TRUNC('year', dc.date);













drop table ImportDevis;
drop table ImportMaison;
drop table ImportPayement;

CREATE TABLE ImportDevis (
    client VARCHAR(50),
    ref_devis VARCHAR(50) PRIMARY KEY,
    type_maison VARCHAR(50),
    finition VARCHAR(50),
    taux_finition VARCHAR(50),
    date_devis VARCHAR(50),
    date_debut VARCHAR(50),
    lieu VARCHAR(50)
);

CREATE TABLE ImportMaison (
    type_maison VARCHAR(50),
    description VARCHAR(255),
    surface VARCHAR(50),
    code_travaux VARCHAR(50),
    type_travaux VARCHAR(50),
    unite VARCHAR(50),
    prix_unitaire VARCHAR(50),
    quantite VARCHAR(50),
    duree_travaux VARCHAR(50)
);

CREATE TABLE ImportPayement (
    ref_devis VARCHAR(50),
    ref_paiement VARCHAR(50),
    date_paiement VARCHAR(50),
    montant VARCHAR(50)
);

