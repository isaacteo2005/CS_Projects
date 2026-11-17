/*
 supabase.js is were backend connects to the cloud database
 The actual database operations are then carried out
  inside the controllers using that Supabase client.
*/

import { createClient } from '@supabase/supabase-js';
import dotenv from 'dotenv';
import path from 'path';
import { fileURLToPath } from 'url';

// Get the directory path
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// Load .env from the parent directory (backend folder)
const envPath = path.join(__dirname, '../.env');
dotenv.config({ path: envPath });

const supabaseUrl = process.env.SUPABASE_URL;
const supabaseKey = process.env.SUPABASE_ANON_KEY;

console.log('\n🔍 Supabase Configuration:');
console.log(`   URL: ${supabaseUrl}`);
console.log(`   Key: ${supabaseKey ? '✅ Loaded (length: ' + supabaseKey.length + ')' : '❌ Missing'}`);

if (!supabaseUrl || !supabaseKey) {
    console.error('\n❌ ERROR: Missing Supabase credentials!');
    process.exit(1);
}

export const supabase = createClient(supabaseUrl, supabaseKey);
console.log('✅ Supabase client initialized successfully\n');