#!/bin/bash
# Script de inicialización para LocalStack
# Crea la cola SQS utilizada por el sistema de turnos

echo "Creando cola SQS: turnos-events..."
awslocal sqs create-queue --queue-name turnos-events
echo "Cola SQS 'turnos-events' creada exitosamente."
