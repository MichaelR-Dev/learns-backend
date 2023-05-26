class Menu {
    depthIndex = 2;

    DisplayTitle = () => {

        console.log(`
        ╔═════════════════════════════════════════════════════════════════════════════╗
        ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
        ║░░░\x1b[33m▓▓▓▓\x1b[0m░░░░░░\x1b[33m▓▓\x1b[0m░░░░░\x1b[33m▓▓▓▓\x1b[0m░░░░\x1b[33m▓▓▓▓\x1b[0m░░░░░░░░░░\x1b[33m▓▓▓▓\x1b[0m░░░\x1b[33m▓▓▓▓▓\x1b[0m░░\x1b[33m▓▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░\x1b[33m▓▓▓▓\x1b[0m░░\x1b[33m▓▓▓▓\x1b[0m░░░║
        ║░░░\x1b[33m▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░░░\x1b[33m▓\x1b[0m░░\x1b[33m▓\x1b[0m░░░░\x1b[33m▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░░░░░░░░░░░░\x1b[33m▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░\x1b[33m▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░\x1b[33m▓\x1b[0m░\x1b[33m▓\x1b[0m░░\x1b[33m▓\x1b[0m░░\x1b[33m▓\x1b[0m░░░░░\x1b[33m▓\x1b[0m░░░░░░║
        ║░░░\x1b[33m▓▓▓▓\x1b[0m░░░░\x1b[33m▓▓▓▓▓▓\x1b[0m░░░\x1b[33m▓▓▓▓\x1b[0m░░░░\x1b[33m▓▓▓▓\x1b[0m░░░\x1b[33m▓▓▓▓\x1b[0m░░░\x1b[33m▓▓▓▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░\x1b[33m▓\x1b[0m░░\x1b[33m▓\x1b[0m░\x1b[33m▓\x1b[0m░░\x1b[33m▓▓▓▓\x1b[0m░░\x1b[33m▓▓▓▓\x1b[0m░░░║
        ║░░░\x1b[33m▓\x1b[0m░░\x1b[33m▓▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░░░\x1b[33m▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░░░░░░░░░░░░\x1b[33m▓\x1b[0m░░\x1b[33m▓▓\x1b[0m░░\x1b[33m▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░\x1b[33m▓\x1b[0m░░░\x1b[33m▓▓\x1b[0m░░\x1b[33m▓\x1b[0m░░░░░░░░\x1b[33m▓\x1b[0m░░░║
        ║░░░\x1b[33m▓▓▓▓\x1b[0m░░░\x1b[33m▓\x1b[0m░░░░░░\x1b[33m▓\x1b[0m░░\x1b[33m▓\x1b[0m░░░░\x1b[33m▓\x1b[0m░░\x1b[33m▓▓▓▓\x1b[0m░░░░░░░░░░\x1b[33m▓▓▓▓\x1b[0m░░░\x1b[33m▓▓▓▓▓\x1b[0m░░\x1b[33m▓\x1b[0m░░░░\x1b[33m▓\x1b[0m░░\x1b[33m▓▓▓▓\x1b[0m░░\x1b[33m▓▓▓▓\x1b[0m░░░║
        ║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║
        ╚═════════════════════════════════════════════════════════════════════════════╝
        \x1b[33m   Barebones is a directory generator using NodeJS and JSON by MichaelR.Dev\x1b[0m
        `)
        
    }

    DisplayTable = async (bones) => {

        console.log('\n');
        console.table(
          bones.map(bone => {
            if(!bone.endsWith('.bb'))
              return;
            return {
              "File_Name": bone,
              "Root_Directory": JSON.parse(fs.readFileSync(`${bonesPath}/${bone}`)).project_name,
              Description: JSON.parse(fs.readFileSync(`${bonesPath}/${bone}`)).description
            };
          })
        );
      
    }

    async SelectBones(bones){
        let selectedIndex = null;
      
        return new Promise((res, rej) => {
          rl.question('\n\nChoose Index: ', index => {
      
            if(isNumeric(index) && typeof bones.at(index) !== "undefined"){
      
              selectedIndex = index;
          
              rl.setPrompt(`\nSelected \x1b[33m${bones.at(selectedIndex)}\x1b[0m\n\nConfirm Selection (Y/N): `);
              rl.prompt();
      
            } else if(index.toLowerCase().localeCompare("exit") == 0){
      
              rl.close()
      
              console.log("\n\x1b[31mExiting Application...\x1b[0m\n")
              process.exit;
      
            } else {
              console.log("\nInvalid Selection, Try Again or Enter 'Exit' to Close Application...");
              DisplayTable(bones);
              rl.setPrompt(`\nChoose Index: `);
              rl.prompt();
            }
          
            rl.on('line', (answer) => {
              if(rl.getPrompt().localeCompare(`\nChoose Index: `) == 0 && isNumeric(answer) && typeof bones.at(answer) !== "undefined"){
                selectedIndex = answer;
        
                rl.setPrompt(`\nSelected \x1b[33m${bones.at(selectedIndex)}\x1b[0m\n\nConfirm Selection (Y/N): `);
                rl.prompt();
              }
          
              if(rl.getPrompt().localeCompare(`\nSelected \x1b[33m${bones.at(selectedIndex)}\x1b[0m\n\nConfirm Selection (Y/N): `) == 0){
                console.log();
                switch (answer.toLowerCase().trim()) {
                  case "y":
                    let mainPath = JSON.parse(fs.readFileSync(`${bonesPath}/${bones.at(selectedIndex)}`)).project_name;
                    generateFolderStructure(mainPath, JSON.parse(fs.readFileSync(`${bonesPath}/${bones.at(selectedIndex)}`)), depthIndex);
          
                    console.log(`\nGenerate structure successful`)
                    rl.close();
                    res();
                    break;
          
                  case "n":
                    DisplayTable(bones);
                    rl.setPrompt(`\nChoose Index: `);
                    rl.prompt();
                    break;
          
                  default:
                    rl.prompt();
                    break;
                }
              } else if(answer.toLowerCase().localeCompare("exit") == 0){
                console.log("\n\x1b[31mExiting Application...\x1b[0m\n")
                process.exit(0);
              }else{
                console.log("\nInvalid Selection, Try Again or Enter 'Exit' to Close Application...");
                DisplayTable(bones);
                rl.setPrompt(`\nChoose Index: `);
                rl.prompt();
              }
            })
          })
        })
    }

    async PromptAnyKey(){

        console.log('\nPress Any Key to Continue...')
      
        process.stdin.setRawMode(true)
        process.stdin.resume();
        return new Promise(resolve => process.stdin.once('data', () => {
          process.stdin.setRawMode(false)
          resolve()
        }))
    }

}

module.exports = Menu;