pipeline {
    agent {
        label 'windows && msbuild && nuget'
    }
    stages {
        stage('Build') {
            steps {
                bat "\"${tool 'MSBuild'}\" Sources\\CoverageConverter.sln -property:Configuration=Release -maxcpucount"
            }
        }
    }
    post {
        always {
            httpRequest outputFile: 'Sources/bin/Release/MSTestCoverageToEmma.xsl', url: "https://github.com/jenkinsci/mstest-plugin/blob/master/src/main/resources/hudson/plugins/mstest/MSTestCoverageToEmma.xsl"
            zip zipFile: 'CoverageConverter.zip', archive: false, dir: 'Sources/bin/Release'
            archiveArtifacts artifacts: 'CoverageConverter.zip', onlyIfSuccessful: true, fingerprint: true
        }
    }
}