CREATE TABLE resources (
  id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name varchar(50) UNIQUE NOT NULL,
  status int NOT NULL
);

CREATE TABLE measures (
  id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name varchar(50) UNIQUE NOT NULL,
  status int NOT NULL
);

CREATE TABLE clients (
  id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name varchar(100) UNIQUE NOT NULL,
  address varchar(255) NOT NULL,
  status int NOT NULL
);

CREATE TABLE balances (
  id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  resource_id int NOT NULL,
  measure_id int NOT NULL,
  quantity int NOT NULL
);

CREATE TABLE documents_receipt (
  id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  number varchar(50) UNIQUE NOT NULL,
  date_receipt date NOT NULL
);

CREATE TABLE receipt_resources (
  id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  document_receipt_id int NOT NULL,
  resource_id int NOT NULL,
  measure_id int NOT NULL,
  quantity int NOT NULL
);

CREATE TABLE documents_shipment (
  id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  number varchar(50) UNIQUE NOT NULL,
  client_id int NOT NULL,
  date_shipment date NOT NULL,
  status int NOT NULL
);

CREATE TABLE shipment_resources (
  id int GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  document_shipment_id int NOT NULL,
  resource_id int NOT NULL,
  measure_id int NOT NULL,
  quantity int NOT NULL
);

ALTER TABLE balances ADD FOREIGN KEY (resource_id) REFERENCES resources (id);

ALTER TABLE balances ADD FOREIGN KEY (measure_id) REFERENCES measures (id);

ALTER TABLE receipt_resources ADD FOREIGN KEY (document_receipt_id) REFERENCES documents_receipt (id);

ALTER TABLE receipt_resources ADD FOREIGN KEY (resource_id) REFERENCES resources (id);

ALTER TABLE receipt_resources ADD FOREIGN KEY (measure_id) REFERENCES measures (id);

ALTER TABLE documents_shipment ADD FOREIGN KEY (client_id) REFERENCES clients (id);

ALTER TABLE shipment_resources ADD FOREIGN KEY (document_shipment_id) REFERENCES documents_shipment (id);

ALTER TABLE shipment_resources ADD FOREIGN KEY (resource_id) REFERENCES resources (id);

ALTER TABLE shipment_resources ADD FOREIGN KEY (measure_id) REFERENCES measures (id);

ALTER TABLE balances
  ADD CONSTRAINT unique_resource_measure UNIQUE (resource_id, measure_id);

CREATE INDEX idx_balances_resource_id ON balances(resource_id);
CREATE INDEX idx_balances_measure_id ON balances(measure_id);

CREATE INDEX idx_receipt_resources_resource_id ON receipt_resources(resource_id);
CREATE INDEX idx_receipt_resources_measure_id ON receipt_resources(measure_id);

CREATE INDEX idx_shipment_resources_resource_id ON shipment_resources(resource_id);
CREATE INDEX idx_shipment_resources_measure_id ON shipment_resources(measure_id);

ALTER TABLE balances ADD CONSTRAINT chk_balances_qty CHECK (quantity >= 0);
ALTER TABLE receipt_resources ADD CONSTRAINT chk_receipt_resources_qty CHECK (quantity > 0);
ALTER TABLE shipment_resources ADD CONSTRAINT chk_shipment_resources_qty CHECK (quantity > 0);

CREATE OR REPLACE VIEW v_receipt_document_resource AS
SELECT
    rr.id AS receipt_resource_id,
    rr.document_receipt_id,
    rr.resource_id,
    r.name AS resource_name,
    rr.measure_id,
    m.name AS measure_name,
    rr.quantity
FROM
    receipt_resources rr
JOIN resources r ON r.id = rr.resource_id
JOIN measures m ON m.id = rr.measure_id;

CREATE OR REPLACE VIEW v_shipment_document_resource AS
SELECT
    sr.id AS shipment_resource_id,
    sr.document_shipment_id,
    sr.resource_id,
    r.name AS resource_name,
    sr.measure_id,
    m.name AS measure_name,
    sr.quantity
FROM
    shipment_resources sr
JOIN resources r ON r.id = sr.resource_id
JOIN measures m ON m.id = sr.measure_id;

CREATE OR REPLACE VIEW v_balance AS
SELECT
    b.resource_id,
    r.name AS resource_name,
    b.measure_id,
    m.name AS measure_name,
    b.quantity AS balance_quantity
FROM balances b
JOIN resources r ON r.id = b.resource_id
JOIN measures m ON m.id = b.measure_id;


