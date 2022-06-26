using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace SpawnItemExample
{
    // This class is using the SpellCastCharge of the game
    public class SpawnItemSpell : SpellCastCharge
    {
        //private EffectData VFXImpactMeteoroidData;
        //private EffectInstance VFXImpactMeteoroidInstance;
        // Happens when you select the spell (with the spellwheel) This is an entry point
        public override void Load(SpellCaster spellCaster, Level level)
        {
            base.Load(spellCaster, level);
            VFXImpactMeteoroidData = Catalog.GetData<EffectData>("TestParticlesMeteoroid");
        }
        // This method is called twice during a cast : the first time while holding the trigger ("active" become true) and the second time when releasing the trigger ("active" become false)
        public override void Fire(bool active)
        {
            base.Fire(active);
            Vector3 forceVect3;
            // This happens while holding the trigger
            if (active)
            {
                //VFXImpactMeteoroidInstance = VFXImpactMeteoroidData.Spawn(Player.local.transform.position + Player.local.transform.forward * 2f, Quaternion.identity);
            }
            // This happens when releasing the trigger
            else
            {
                // if at 10% of the charge of the spell (1 = max charge (100%), 0 no charge (0%), 0.5 half the charge (50%))
                if (currentCharge >= 0.1f)
                {
                    //forceVect3 = Player.local.head.transform.forward;
                    forceVect3 = -spellCaster.ragdollHand.transform.forward.normalized;
                    //SpawnItem(forceVect3);
                    Throw(forceVect3 * 200f * currentCharge);
                }
                //VFXImpactMeteoroidInstance.Stop();
            }
        }
        // Happens constantly no matter whether you're charging or not
        public override void UpdateCaster()
        {
            base.UpdateCaster();
        }
        // Happens when you deselect the spell (with the spellwheel)
        public override void Unload()
        {
            base.Unload();
        }

        public override void Throw(Vector3 velocity)
        {
            base.Throw(velocity);
            SpawnItem(velocity);
        }

        // This method spawn a spikes 1 meter on top of the hand
        private void SpawnItem(Vector3 velocitySpawn)
        {

            /* Spell 1 */
            // This is spawning a brick at 1 meter on top of the hand
            /*
            Catalog.GetData<ItemData>("StoneCitadel4") // the id for the brick as defined in its JSON which is of type ItemData, which inherits from CatalogData
                .SpawnAsync(null, (spellCaster.ragdollHand.grip.position + 1f * Vector3.up), spellCaster.ragdollHand.grip.rotation);
            */

            /* Spell 2 */
            // This is spawning a sphere at 1 meter from the hand of the 
            Catalog.GetData<ItemData>("StatueSphere"/*the id for the spikes as defined in its JSON which is of type ItemData, which inherits from CatalogData*/)
                .SpawnAsync(projectile =>
                {
                    // Set the position of the item :
                    // The position of the grip of hand ; "spellCaster.ragdollHand.grip.position"
                    // Then I want to avoid the item touching the hand when spawning so I add a vector ; "+"
                    // to make it spawn 1 meter farther the grip of hand in the direction the palm ; "spellCaster.ragdollHand.transform.forward * -1f"
                    projectile.transform.position = spellCaster.ragdollHand.transform.position + spellCaster.ragdollHand.transform.forward * -1f;
                    // Next I want to add the collision and the damager to my item so it can collides and deal damage
                    foreach (CollisionHandler collisionHandler in projectile.collisionHandlers)
                    {
                        // Load the damager from the catalog, you can put any damager you like (a ball that deal lightning damage or fireball damage)
                        foreach (Damager damager in collisionHandler.damagers)
                            damager.Load(Catalog.GetData<DamagerData>("PropBluntHeavy"), collisionHandler);
                    }
                    // Add some force to the object in the direction you want
                    projectile.rb.AddForce(velocitySpawn, ForceMode.VelocityChange);
                    // Add this line to make the object damage enemy, else it'll just fly and bounce on enemy without doing damages
                    projectile.Throw(flyDetection: Item.FlyDetection.Forced);
                    // Despawn the projectile after 5 seconds
                    projectile.Despawn(5f);
                }, spellCaster.ragdollHand.transform.position + spellCaster.ragdollHand.transform.forward * -1f);
        }
    }
}
