CREATE TABLE torneos (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    edicion INT NOT NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    creado_en TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


CREATE TABLE equipos (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    torneo_id BIGINT NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    ciudad VARCHAR(100) NOT NULL,

    CONSTRAINT fk_equipo_torneo
        FOREIGN KEY (torneo_id)
        REFERENCES torneos(id)
        ON DELETE RESTRICT
);


CREATE TABLE partidos (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    torneo_id BIGINT NOT NULL,
    equipo_local_id BIGINT NOT NULL,
    equipo_visitante_id BIGINT NOT NULL,
    goles_local INT DEFAULT 0,
    goles_visitante INT DEFAULT 0,
    fecha_partido TIMESTAMP NOT NULL,
    jugado BOOLEAN DEFAULT FALSE,
    ronda int DEFAULT 1,
    ganador_id int NULL,

    CONSTRAINT fk_partido_torneo
        FOREIGN KEY (torneo_id)
        REFERENCES torneos(id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_partido_local
        FOREIGN KEY (equipo_local_id)
        REFERENCES equipos(id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_partido_visitante
        FOREIGN KEY (equipo_visitante_id)
        REFERENCES equipos(id)
        ON DELETE RESTRICT,

    CONSTRAINT chk_equipos_diferentes
        CHECK (equipo_local_id <> equipo_visitante_id)
);


INSERT INTO torneos (nombre, edicion, activo)
VALUES
('Torneo de memes', 2026, True);

INSERT INTO equipos (torneo_id, nombre, ciudad)
VALUES
(1, 'FC Crustáceo Cascarudo', 'Fondo de Bikini'),
(1, 'Shrek United', 'Pantano'),
(1, 'Real Michis', 'Miau City'),
(1, 'Los Trolls', 'Funky Town'),
(1, 'Hackermen', 'Deep Web'),
(1, 'Los Pepes', 'Internet'),
(1, 'Los Minions', 'Gru City'),
(1, 'Breaking Bad FC', 'Albuquerque');


INSERT INTO torneos (nombre, edicion, activo)
VALUES
('Champions League', 2026, TRUE);

INSERT INTO equipos (torneo_id, nombre, ciudad)
VALUES
(2, 'Real Madrid', 'Madrid'),
(2, 'FC Barcelona', 'Barcelona'),
(2, 'Manchester City', 'Manchester'),
(2, 'Liverpool', 'Liverpool'),
(2, 'Bayern Munich', 'Múnich'),
(2, 'Paris Saint-Germain', 'París'),
(2, 'Inter Milan', 'Milán'),
(2, 'Ajax', 'Ámsterdam');


INSERT INTO torneos (nombre, edicion, activo)
VALUES
('Torneo de verano', 2026, TRUE);

INSERT INTO equipos (torneo_id, nombre, ciudad)
VALUES
(3, 'Deportivo Saprissa', 'San José'),
(3, 'Liga Deportiva Alajuelense', 'Alajuela'),
(3, 'Club Sport Herediano', 'Heredia'),
(3, 'Club Sport Cartaginés', 'Cartago'),
(3, 'Puntarenas FC', 'Puntarenas'),
(3, 'Municipal Liberia', 'Liberia'),
(3, 'Sporting FC', 'San José'),
(3, 'Santos de Guápiles', 'Guápiles');


INSERT INTO torneos (nombre, edicion, activo)
VALUES
('Torneo Barrios San Ramón', 2026, TRUE);

INSERT INTO equipos (torneo_id, nombre, ciudad)
VALUES
(4, 'Las Panteras' , 'San Pedro'),
(4, 'Los Toros', 'Bolívar'),
(4, 'Los Halcones', 'San Rafael'),
(4, 'Los Tigres', 'San Juan'),
(4, 'Las Águilas', 'Piedades Sur'),
(4, 'Los Guerreros', 'Concepción'),
(4, 'Los Leones', 'San Isidro'),
(4, 'Los Lobos', 'Piedades Sur');