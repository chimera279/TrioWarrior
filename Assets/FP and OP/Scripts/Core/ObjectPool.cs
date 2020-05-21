using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{

    #region ObjectPoolSingleton
    private static ObjectPool instance = null;

    private ObjectPool()
    {
    }

    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
                instance = new ObjectPool();

            return instance;
        }
    }
    #endregion

    public Dictionary<Attacks.AtkDefElement, Queue<RPSEnemy>> EnemyPool = new Dictionary<Attacks.AtkDefElement, Queue<RPSEnemy>>();
    public Dictionary<EnvironmentType, Queue<EnvironmentObject>> EnvObjPool = new Dictionary<EnvironmentType, Queue<EnvironmentObject>>();
    public Dictionary<Attacks.AttackType, Queue<ParticleSystem>> ParticlePool = new Dictionary<Attacks.AttackType, Queue<ParticleSystem>>();
    public Queue<AudioSource> AudioPool = new Queue<AudioSource>();
    public void PoolBug(Attacks.AtkDefElement elemtype, RPSEnemy enemy)
    {
        if (!EnemyPool.ContainsKey(elemtype))
            EnemyPool.Add(elemtype, new Queue<RPSEnemy>());
        EnemyPool[elemtype].Enqueue(enemy);
    }
    public bool HasBug(Attacks.AtkDefElement elemtype)
    {
        if (EnemyPool.ContainsKey(elemtype))
            if (EnemyPool[elemtype].Count > 0)
                return true;
        return false;
    }
    public RPSEnemy DepoolBug(Attacks.AtkDefElement elemtype)
    {
        if (EnemyPool.ContainsKey(elemtype))
            if (EnemyPool[elemtype].Count > 0)
                return EnemyPool[elemtype].Dequeue();
        return null;
    }

    public void PoolObject(EnvironmentType elemtype, EnvironmentObject envObject)
    {
        if (!EnvObjPool.ContainsKey(elemtype))
            EnvObjPool.Add(elemtype, new Queue<EnvironmentObject>());
        EnvObjPool[elemtype].Enqueue(envObject);
    }
    public bool HasObject(EnvironmentType elemtype)
    {
        if (EnvObjPool.ContainsKey(elemtype))
            if (EnvObjPool[elemtype].Count > 0)
                return true;
        return false;
    }
    public EnvironmentObject DepoolObject(EnvironmentType elemtype)
    {
        if (EnvObjPool.ContainsKey(elemtype))
            if (EnvObjPool[elemtype].Count > 0)
                return EnvObjPool[elemtype].Dequeue();
        return null;
    }

    public void PoolParticle(Attacks.AttackType elemtype, ParticleSystem particle)
    {
        if (!ParticlePool.ContainsKey(elemtype))
            ParticlePool.Add(elemtype, new Queue<ParticleSystem>());
        ParticlePool[elemtype].Enqueue(particle);
    }
    public bool HasParticle(Attacks.AttackType elemtype)
    {
        if (ParticlePool.ContainsKey(elemtype))
            if (ParticlePool[elemtype].Count > 0)
                return true;
        return false;
    }
    public ParticleSystem DepoolParticle(Attacks.AttackType elemtype)
    {
        if (ParticlePool.ContainsKey(elemtype))
            if (ParticlePool[elemtype].Count > 0)
                return ParticlePool[elemtype].Dequeue();
        return null;
    }

    public void PoolAudio( AudioSource audio)
    {
        AudioPool.Enqueue(audio);
    }
    public bool HasAudio()
    {
       if (AudioPool.Count > 0)
                return true;
        return false;
    }
    public AudioSource DepoolAudio()
    {
            if (AudioPool.Count > 0)
                return AudioPool.Dequeue();
        return null;
    }




}
