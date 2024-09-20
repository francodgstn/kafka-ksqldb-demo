# demo.ps1

# Step 1: Start Docker containers
docker-compose down
Write-Host "Starting Docker containers..."
docker-compose up -d

# Step 4: Wait for the producer to start sending messages
Write-Host "Waiting for the environment to finish setting up..."
Start-Sleep -Seconds 3

# Step 2: Create Kafka topic
Write-Host "Creating Kafka topic 'flights'..."
docker exec -it broker kafka-topics --create --topic flights --bootstrap-server broker:9092 --partitions 1 --replication-factor 1

# Step 3: Run the .NET producer application
Write-Host "Running the .NET producer application..."
Start-Process "dotnet" -ArgumentList "run" -WorkingDirectory "./KafkaFlightProducer"