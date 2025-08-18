pipeline {
    agent any
    environment {
        SERVICE_NAME = 'test'
        ENV_NAME = 'production'
        DOCKER_IMAGE_NAME = 'nguyenphu/testperformance'
        DOCKER_CREDENTIALS = credentials('docker-hub-credentials') // Add this credential in Jenkins
    }
    options {
        timeout(time: 1, unit: 'HOURS')
    }
    stages {
        stage('Validate Branch') {
            steps {
                echo 'Checking branch...'
                script {
                    def branch = sh(script: 'git rev-parse --abbrev-ref HEAD', returnStdout: true).trim()
                    echo "Current branch: ${branch}"
                    if (branch != 'Dev') {
                        error "This pipeline can only run on the 'Dev' branch. Current branch: ${branch}"
                    }
                }
            }
        }
        
        stage('Build') {
            steps {
                echo 'Building the application...'
                script {
                    // Get the commit hash for tagging
                    def gitCommit = sh(script: 'git rev-parse --short HEAD', returnStdout: true).trim()
                    def dockerTag = "v${gitCommit}"
                    
                    // Build Docker image
                    sh "docker build -t ${DOCKER_IMAGE_NAME}:${dockerTag} ."
                    sh "docker tag ${DOCKER_IMAGE_NAME}:${dockerTag} ${DOCKER_IMAGE_NAME}:latest"
                }
            }
        }
        
        stage('Test') {
            steps {
                echo 'Running tests...'
                // Add your test commands here
                sh 'dotnet test --no-build'
            }
        }
        
        stage('Publish Docker Image') {
            steps {
                echo 'Publishing Docker image to registry...'
                script {
                    // Login to Docker registry
                    sh 'docker login -u $DOCKER_CREDENTIALS_USR -p $DOCKER_CREDENTIALS_PSW'
                    
                    // Get the commit hash for tagging
                    def gitCommit = sh(script: 'git rev-parse --short HEAD', returnStdout: true).trim()
                    def dockerTag = "v${gitCommit}"
                    
                    // Push both tagged and latest images
                    sh "docker push ${DOCKER_IMAGE_NAME}:${dockerTag}"
                    sh "docker push ${DOCKER_IMAGE_NAME}:latest"
                    
                    // Logout for security
                    sh 'docker logout'
                }
            }
        }
        
        stage('Deploy') {
            steps {
                echo 'Deploying the application...'
                // Add your deployment commands here
            }
        }
    }
    
    post {
        always {
            // Clean up any dangling images
            sh 'docker system prune -f'
        }
    }
}
