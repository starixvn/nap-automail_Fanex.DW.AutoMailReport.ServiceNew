@Library("JenkinsPipelineLibrary") _

pipeline {
  agent any

  options{
    buildDiscarder(logRotator(numToKeepStr: '5'))    
    disableConcurrentBuilds()
  }

  triggers{
    pollSCM("H/15 * * * *")
  } 

  stages {
    stage('Build') {
      steps {        
        script{
          pipelineLib.beginSonarQubeForMsBuild("DW-AutoMail-ServiceNew", "Olympus / DW-AutoMail-ServiceNew", "/d:sonar.cs.opencover.reportsPaths=%WORKSPACE%/coverage.xml /d:sonar.cs.nunit.reportsPaths=%WORKSPACE%/UnitTest.xml")

          pipelineLib.restoreNuget()

          pipelineLib.msBuild("Fanex.DW.AutoMailReport.ServiceNew.sln")
        }
      }
    }
   
     stage("Test"){
      parallel{
        stage("C# Unit Test"){
          steps{
            script{
              pipelineLib.nunit()
            }
          }
        }       
      }      
    }    

    stage("SonarQube Analysis"){
      steps{       
        script{
          pipelineLib.endSonarQubeForMsBuild()
        }
      }
      
      post{
        always{
          script{
            if(manager.logContains(".*Quality gate status.*")){              
              pipelineLib.generateSonarQubeReport("DW-AutoMail-ServiceNew")
            }
          }
        }
      }
    }
  }

  post{
    always{
      archiveArtifacts "*.xml,*.email"
      step([$class: 'ACIPluginPublisher', name: '*.xml', shownOnProjectPage: false])
      cobertura coberturaReportFile: "cobertura.xml", onlyStable: false
      nunit testResultsPattern: 'UnitTest.xml'
    }
    unsuccessful{
      emailext body: '$DEFAULT_CONTENT', subject: '$DEFAULT_SUBJECT', to: 'robert.tran@starixsoft.com'
    }
  }
}