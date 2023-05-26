//External Imports
const fs = require('fs');
const readline = require('readline');
const rl = readline.createInterface({input: process.stdin, output: process.stdout});

//Internal Imports
const Menu = require('./menu');

let args = process.argv[0].split('\\')
args[args.length - 1] = "project_bones";
const bonesPath = args.join('\\');



const generateFolderStructure = (path, structure, depthIndex) => {

  if (!fs.existsSync(path)) {
    fs.mkdirSync(path);
  }

  try {
    Object.keys(structure).forEach((key) => {
      if(key.toLocaleLowerCase() == "description")
        return;
  
      const value = structure[key];
      const newPath = `${path}/${key}`;
  
      if (key.toLocaleLowerCase() == "project_name") {
  
        console.log(` ${"└─▼"}Writing "\x1b[32m${path}\x1b[0m" directory...` )
  
      } else if(typeof value === 'object' && key.toLocaleLowerCase() != "project_name") {

        console.log(` ${" ".repeat(depthIndex)+"└─▼"}Writing "\x1b[32m${key}\x1b[0m" directory...` )

        generateFolderStructure(newPath, value, depthIndex + 2);

      } else if (key.toLocaleLowerCase() != "project_name"){
  
        console.log(` ${" ".repeat(depthIndex)+"├─>"}Writing "\x1b[33m${key}\x1b[0m" file...` )
        fs.writeFileSync(newPath, value);
  
      }
    });
  } catch (error) {
    console.log(error);
  }

};

function isNumeric(str) {
  if (typeof str != "string") return false // we only process strings!  
  return !isNaN(str) && // use type coercion to parse the _entirety_ of the string (`parseFloat` alone does not do this)...
         !isNaN(parseFloat(str)) // ...and ensure strings of whitespace fail
}

const main = async () => {

  const menu = new Menu();

  if(!fs.existsSync(bonesPath)){
    console.error("\nFAILURE TO EXECUTE\n\n\x1b[31mERROR\x1b[0m: \x1b[33mMissing '.bb' files...\x1b[0m\nEnsure there is a 'project_bones' directory containing at least one '.bb' file in the barebones.exe root folder!");
    await menu.PromptAnyKey();
  }

  const bones = fs.readdirSync(bonesPath);
  
  try{
    if(fs.readdirSync(bonesPath).length == 0){
      console.error("\nFAILURE TO EXECUTE\n\n\x1b[31mERROR\x1b[0m: \x1b[33mMissing '.bb' files...\x1b[0m\nEnsure there is a 'project_bones' directory containing at least one '.bb' file in the barebones.exe root folder!");
      await menu.PromptAnyKey();
    }else{
      menu.DisplayTitle();
      menu.DisplayTable(bones);
      
      await SelectBones(bones);
      await menu.PromptAnyKey();
    }
  }catch(err){
    console.log(err);
    await menu.PromptAnyKey();
  }
}

main()
.then(()=>{
  console.log("\n\x1b[31mExiting Application...\x1b[0m\n")
  process.exit(0);
})