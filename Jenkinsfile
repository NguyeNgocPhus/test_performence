pipeline {
    agent any
    environment {
        PATH = "/usr/local/bin:/opt/homebrew/bin:/usr/bin:/bin:/usr/sbin:/sbin"
        SERVICE_NAME = 'test'
        ENV_NAME = 'production'
        DOCKER_IMAGE_NAME = 'nguyenphu/testperformance'
        DOCKER_CREDENTIALS = credentials('docker-hub-credentials') // Add this credential in Jenkins
    }
    options {
        timeout(time: 1, unit: 'HOURS')
    }
    stages {
        // stage('Validate Branch') {
        //     steps {
        //         echo 'Checking branch...'
        //         script {
        //             env.BRANCH_NAME = sh(script: 'git rev-parse --abbrev-ref HEAD', returnStdout: true).trim()
        //             echo "Current branch: ${env.BRANCH_NAME}"
        //             if (env.BRANCH_NAME != 'Dev') {
        //                 error "This pipeline can only run on the 'Dev' branch. Current branch: ${env.BRANCH_NAME}"
        //             }
        //         }
        //     }
        // }
        stage('Check Docker') {
            steps {
                sh '''
                docker version
                '''
            }
        }
        stage('Build') {
            steps {
                echo 'Building the application...'
                script {
                    // Get the commit hash for tagging
                    env.GIT_COMMIT = sh(script: "git log -n 1 --pretty=format:'%H'", returnStdout: true).trim()
                    env.DOCKER_TAG = "${env.GIT_COMMIT.take(7)}"
                    echo "Short commit hash = ${env.DOCKER_TAG}"
                    // Build Docker image
                    sh "docker build -t ${DOCKER_IMAGE_NAME}:${env.DOCKER_TAG} ."
                }
            }
        }
    
        
        stage('Publish Docker Image') {
            steps {
                echo 'Publishing Docker image to registry...'
                script {
                    // Login to Docker registry
                    sh 'docker login -u $DOCKER_CREDENTIALS_USR -p $DOCKER_CREDENTIALS_PSW'
                
                    // Push both tagged and latest images
                    sh "docker push ${DOCKER_IMAGE_NAME}:${env.DOCKER_TAG}"
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

}
