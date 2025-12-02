-- Arregla los datos de la tabla ot
UPDATE otprueba o
JOIN ordentrabajo x ON o.id = x.id_ot
SET 
    o.clienteproporcionapelicula = (x.pelicula = 'X'),
    o.clienteproporcionaplancha  = (x.plancha = 'X'),
    o.clienteproporcionapapel    = (x.papel = 'X');


UPDATE otprueba o
JOIN ordentrabajo t ON o.id = t.id
SET
    o.tintas1 = CASE 
                   WHEN t.tintas1 IN ('1', '2', '3') THEN t.tintas1
                   ELSE '3'
                END;

-- Crear Tabla tipo_tinta (para reporteria futura)
CREATE TABLE tipo_tinta(
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(255) NOT NULL);

INSERT INTO tipo_tinta(nombre) VALUES
('Tricromía'),
('Pantone'),
('S/Muestra');