

module.exports = function (grunt) {
    'use strict';    

    const sass = require('node-sass');   
    require('load-grunt-tasks')(grunt);   
    const path = require('path');

    grunt.initConfig({       
        sass: {           
            options: {                
                implementation: sass,         
                sourceMap: true,             
                outputStyle: 'compressed',
                includePaths: [
                    path.join(path.dirname(module.filename), 'node_modules')
                ]
            },
            dist: {
                files: [
                    {                      
                        "wwwroot/lib/mtd-ordermaker/common/css/mtd-desk.css" : "wwwroot/lib/mtd-ordermaker/common/sass/mtd-desk.scss",                        
                    }
                ]
            }
        }
    });
    
    grunt.registerTask('default', ['sass']);
};
