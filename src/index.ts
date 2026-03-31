#!/usr/bin/env node
import inquirer from 'inquirer'
import fs from 'fs'
import path from 'path'
import { execSync } from 'child_process'
import { fileURLToPath } from 'url'

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)
const TEMPLATE_PATH = path.resolve(__dirname, '..', 'SurvivalcraftMod')

function copyDir(src: string, dest: string): void {
  if (!fs.existsSync(src)) return
  fs.mkdirSync(dest, { recursive: true })

  const entries = fs.readdirSync(src, { withFileTypes: true })

  for (const entry of entries) {
    const srcPath = path.join(src, entry.name)
    const destPath = path.join(dest, entry.name)

    if (entry.isDirectory()) {
      copyDir(srcPath, destPath)
    } else {
      fs.copyFileSync(srcPath, destPath)
    }
  }
}

function replaceInFile(filePath: string, replacements: [string, string][]): void {
  let content = fs.readFileSync(filePath, 'utf8')
  for (const [oldStr, newStr] of replacements) {
    content = content.split(oldStr).join(newStr)
  }
  fs.writeFileSync(filePath, content, 'utf8')
}

function renameFile(dir: string, oldName: string, newName: string): void {
  const oldPath = path.join(dir, oldName)
  const newPath = path.join(dir, newName)
  if (fs.existsSync(oldPath)) {
    fs.renameSync(oldPath, newPath)
  }
}

interface ModInfo {
  Name: string
  [key: string]: unknown
}

async function createProject(projectName: string): Promise<void> {
  const projectPath = path.join(process.cwd(), projectName)

  if (fs.existsSync(projectPath)) {
    console.error(`Error: Directory "${projectName}" already exists.`)
    process.exit(1)
  }

  console.log(`Copying template from ${TEMPLATE_PATH}...`)
  copyDir(TEMPLATE_PATH, projectPath)
  console.log(`✓ Copied template to: ${projectName}`)

  const replacements: [string, string][] = [
    ['SurvivalcraftMod', projectName]
  ]

  replaceInFile(path.join(projectPath, 'SurvivalcraftMod.sln'), replacements)
  renameFile(projectPath, 'SurvivalcraftMod.sln', `${projectName}.sln`)
  console.log(`✓ Renamed: SurvivalcraftMod.sln → ${projectName}.sln`)

  const csprojPath = path.join(projectPath, 'src', 'SurvivalcraftMod.csproj')
  replaceInFile(csprojPath, [
    ['SurvivalcraftMod', projectName],
    ['<AssemblyName>SurvivalcraftMod</AssemblyName>', `<AssemblyName>${projectName}</AssemblyName>`]
  ])
  renameFile(path.join(projectPath, 'src'), 'SurvivalcraftMod.csproj', `${projectName}.csproj`)
  console.log(`✓ Renamed: src/SurvivalcraftMod.csproj → src/${projectName}.csproj`)

  const modinfoPath = path.join(projectPath, 'src', 'modinfo.json')
  const modinfo: ModInfo = JSON.parse(fs.readFileSync(modinfoPath, 'utf8'))
  modinfo.Name = projectName
  fs.writeFileSync(modinfoPath, JSON.stringify(modinfo, null, 2), 'utf8')
  console.log(`✓ Updated: modinfo.json (Name: ${projectName})`)

  const class1Path = path.join(projectPath, 'src', 'Class1.cs')
  if (fs.existsSync(class1Path)) {
    replaceInFile(class1Path, [
      ['namespace SurvivalcraftMod', `namespace ${projectName}`]
    ])
    console.log(`✓ Updated: Class1.cs namespace`)
  }

  renameFile(projectPath, 'SurvivalcraftMod.DotSettings', `${projectName}.DotSettings`)
  console.log(`✓ Renamed: SurvivalcraftMod.DotSettings → ${projectName}.DotSettings`)

  const answers = await inquirer.prompt([
    {
      type: 'confirm',
      name: 'gitInit',
      message: 'Do you want to initialize git repository?',
      default: true
    }
  ])

  if (answers.gitInit) {
    const gitignoreSrc = path.join(TEMPLATE_PATH, '.gitignore')
    const gitignoreDest = path.join(projectPath, '.gitignore')

    if (fs.existsSync(gitignoreSrc)) {
      fs.copyFileSync(gitignoreSrc, gitignoreDest)
      console.log('✓ Copied: .gitignore')
    }

    try {
      execSync('git init', { cwd: projectPath, stdio: 'ignore' })
      console.log('✓ Initialized git repository')
    } catch (e) {
      console.log('⚠ git init failed (git may not be installed)')
    }
  }

  console.log(`\n✓ Project "${projectName}" created successfully!`)
}

const args = process.argv.slice(2)
if (args[0] === 'new' && args[1]) {
  createProject(args[1])
} else {
  console.log('Usage: scmod new <project-name>')
  console.log('Example: scmod new my-mod')
}
