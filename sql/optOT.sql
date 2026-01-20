CREATE INDEX idx_ot_empresa_fecha
ON ordentrabajo (empresa, fecha DESC);

CREATE INDEX idx_ot_rutcliente
ON ordentrabajo (rutcliente);

CREATE INDEX idx_ot_maquina1
ON ordentrabajo (maquina1);

CREATE INDEX idx_cliente_rut
ON cliente (rut);

CREATE INDEX idx_maquina_codigo
ON maquina (codigo);

PRIMARY KEY (id)

CREATE OR REPLACE
ALGORITHM = MERGE
DEFINER = root@localhost
SQL SECURITY DEFINER
VIEW vw_ordentrabajo AS
SELECT 
    t.id,
    t.folio,
    t.rutcliente,
    t.fecha,
    t.descripcion,
    t.cantidad,
    t.totalimpresion,
    t.foliodesde,
    t.foliohasta,
    t.cortartamanio,
    t.cortartamanion,
    t.cortartamaniolargo,
    t.montar,
    t.moldetamanio,
    t.tamaniofinalancho,
    t.tamaniofinallargo,
    t.clienteproporcionanada,
    t.clienteproporcionaoriginal,
    t.clienteproporcionapelicula,
    t.clienteproporcionaplancha,
    t.clienteproporcionapapel,
    t.tipoimpresion,
    t.maquina1,
    t.maquina2,
    t.pin,
    t.nva,
    t.us,
    t.ctpnva,
    t.u,
    t.sobres,
    t.sacos,
    t.tintas1,
    t.tintas2,
    t.tintas3,
    t.tintas4,
    t.fechaentrega,
    t.empresa,

    e.nombre        AS EmpresaNombre,
    c.razon_social  AS razon_social,
    m.descripcion  AS maquina1descripcion

FROM ordentrabajo t
LEFT JOIN empresa e
    ON t.empresa = e.id
LEFT JOIN cliente c
    ON t.rutcliente = c.rut
LEFT JOIN maquina m
    ON t.maquina1 = m.codigo;
