ALTER TABLE `usuario`
DROP COLUMN `tipo`;

ALTER TABLE `usuario`
ADD COLUMN `tipo` BIGINT NULL AFTER `nombre`;


ALTER TABLE usuario
ADD CONSTRAINT fk_usuario_tipo
FOREIGN KEY (id_tipo)
REFERENCES usuario_tipo(id_tipo);

insert into tipo_usuario(nombre_tipo) values ("usuario")

update usuario
    set tipo = 1
    where id = 1;
    
update usuario
    set tipo = 1
    where id = 2;
    
update usuario
    set tipo = 2
    where id = 7;